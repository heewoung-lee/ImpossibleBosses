using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SocialPlatforms;

public class Indicator_Controller : NetworkBehaviourBase
{

    private const float DEPTH = 10f;

    enum DecalProjectors
    {
        Circle,
        CircleBorder
    }

    Action doneEvent;

    [SerializeField]
    private NetworkVariable<float> _radius = new NetworkVariable<float>
        (0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField]
    private NetworkVariable<float> _angle = new NetworkVariable<float>
        (0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField]
    private NetworkVariable<float> _arc = new NetworkVariable<float>
        (0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField]
    private NetworkVariable<Vector3> _callerPosition = new NetworkVariable<Vector3>
        (Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private DecalProjector _decal_Circle_projector;
    private DecalProjector _decal_CircleBorder_projector;

    public float Radius
    {
        get => _radius.Value;

        private set
        {
            if (IsServer == false) return;
            _radius.Value = Mathf.Max(value, 0f);
        }
    }
    public float Angle
    {
        get => _angle.Value;
        private set
        {
            if (IsServer == false) return;
            _angle.Value = value;
        }
    }
    public float Arc
    {
        get => _arc.Value;
        private set
        {
            if (IsServer == false) return;
            _arc.Value = Mathf.Clamp(value, 0f, 360f);
        }
    }
    public Vector3 CallerPosition
    {
        get => _callerPosition.Value;
        private set
        {
            if (IsServer == false) return;
            _callerPosition.Value = value;
        }
    }

    private static readonly int ColorShaderID = Shader.PropertyToID("_Color");

    private static readonly int FillColorShaderID = Shader.PropertyToID("_FillColor");

    private static readonly int FillProgressShaderID = Shader.PropertyToID("_FillProgress");

    private static readonly int ArcShaderID = Shader.PropertyToID("_Arc");

    private static readonly int AngleShaderID = Shader.PropertyToID("_Angle");

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SubscribeValueEvents();

        OnCallerPositionChanged(Vector3.zero, CallerPosition);
        OnRadiusValueChanged(0f, Radius);
        OnArcValueChagned(0f, Arc);
        OnAngleValueChanged(0f, Angle);
        StartCoroutine(Play_Indicator());
    }

    private void SubscribeValueEvents()
    {
        _radius.OnValueChanged += OnRadiusValueChanged;
        _arc.OnValueChanged += OnArcValueChagned;
        _angle.OnValueChanged += OnAngleValueChanged;
        _callerPosition.OnValueChanged += OnCallerPositionChanged;
    }
    private void OnArcValueChagned(float previousValue, float newValue)
    {
        float arcAngleNormalized = 1f - newValue / 360;
        _decal_Circle_projector.material.SetFloat(ArcShaderID, arcAngleNormalized);
        _decal_CircleBorder_projector.material.SetFloat(ArcShaderID, arcAngleNormalized);
    }
    private void OnCallerPositionChanged(Vector3 previousValue, Vector3 newValue)
    {
        transform.position = newValue;
    }
    private void OnRadiusValueChanged(float previousValue, float newValue)
    {
        Vector3 currentSize;
        currentSize.x = newValue * 2; //_radius는 반지름의 길이 이므로 Project의 크기는 2배로 키워야함
        currentSize.y = newValue * 2;
        currentSize.z = DEPTH;

        _decal_Circle_projector.size = currentSize;
        _decal_CircleBorder_projector.size = currentSize;
    }

    private void OnAngleValueChanged(float previousValue, float newValue)
    {
        float normalizedAngle = Mathf.Repeat((newValue - 90) % 360, 360) / 360;
        _decal_CircleBorder_projector.material.SetFloat(AngleShaderID, normalizedAngle);
        _decal_Circle_projector.material.SetFloat(AngleShaderID, normalizedAngle);
    }
    protected override void StartInit()
    {
    }

    protected override void AwakeInit()
    {
        Bind<DecalProjector>(typeof(DecalProjectors));
        _decal_Circle_projector = Get<DecalProjector>((int)DecalProjectors.Circle);
        _decal_CircleBorder_projector = Get<DecalProjector>((int)DecalProjectors.CircleBorder);
        GetComponent<Poolable>().WorldPositionStays = false;
        GetComponent<NGO_PoolingInitalize_Base>().PoolObjectReleaseEvent += ReleseProjector;
        ReassignMaterials();
    }

    private void ReassignMaterials()
    {
        if (_decal_Circle_projector != null)
            _decal_Circle_projector.material = new Material(_decal_Circle_projector.material);

        if (_decal_CircleBorder_projector != null)
            _decal_CircleBorder_projector.material = new Material(_decal_CircleBorder_projector.material);
    }

    private void UpdateDecalFillProgressProjector(float fillAmount)
    {
        _decal_Circle_projector.material.SetFloat(FillProgressShaderID, fillAmount);
        _decal_CircleBorder_projector.material.SetFloat(FillProgressShaderID, fillAmount);
    }
    public void SetValue(float radius, float arc, Transform callerTr, Action anotherOption = null)
    {
        Radius = radius;
        Arc = arc;
        CallerPosition = callerTr.position;
        Angle = callerTr.eulerAngles.y;
        doneEvent += anotherOption;
    }

    IEnumerator Play_Indicator()
    {
        float fillAmount = 0f;
        while (fillAmount < 1)
        {
            yield return null;
            fillAmount = Mathf.Clamp01(fillAmount += Time.deltaTime * 0.45f);
            UpdateDecalFillProgressProjector(fillAmount);
        }
        doneEvent?.Invoke();
        Managers.ResourceManager.DestroyObject(gameObject);
    }


    private void ReleseProjector()
    {
        Debug.Log("이거 찍힘?");
        CallerPosition = Vector3.zero;
        Radius = 0f;
        Arc = 0f;
        Angle = 0f;
        UpdateDecalFillProgressProjector(0f);
        transform.position = Vector3.zero;
    }
}
