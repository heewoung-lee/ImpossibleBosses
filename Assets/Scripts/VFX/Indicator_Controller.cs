using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SocialPlatforms;

public class Indicator_Controller : NetworkBehaviourBase
{
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
    private NetworkVariable<float> _fillProgress = new NetworkVariable<float>
        (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField]
    private NetworkVariable<Vector3> _callerPosition = new NetworkVariable<Vector3>
        (Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField]
    private NetworkVariable<Quaternion> _callerLotation = new NetworkVariable<Quaternion>
    (Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private DecalProjector _decal_Circle_projector;
    private DecalProjector _decal_CircleBorder_projector;

    public float Radius
    {
        get => _radius.Value;

        set
        {
            if (IsHost == false) return;
            _radius.Value = Mathf.Max(value, 0f);
        }
    }
    public float Angle
    {
        get => _angle.Value;
        set
        {
            if (IsHost == false) return;
            _angle.Value = value;
        }
    }
    public float Arc
    {
        get => _arc.Value;
        set
        {
            if (IsHost == false) return;
            _arc.Value = Mathf.Clamp(value, 0f, 360f);
        }
    }
    public float FillProgress
    {
        get => _fillProgress.Value;
        set
        {
            if (IsHost == false) return;
            _fillProgress.Value = value;
            _decal_Circle_projector.material.SetFloat(FillProgressShaderID, _fillProgress.Value);
        }
    }
    public Vector3 CallerPosition
    {
        get => _callerPosition.Value;

        set
        {
            if (IsHost == false) return;
            _callerPosition.Value = value;
        }
    }
    public Quaternion CallerLocation
    {
        get
        {
            return _callerLotation.Value;
        }
        set
        {
            if (IsHost == false) return;
            _callerLotation.Value = value;
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
        UpdateProjectors();
        _fillProgress.OnValueChanged += ONChangedFillProgressValue;
        _callerPosition.OnValueChanged += ONChangedCallerPositionValue;
        _callerLotation.OnValueChanged += ONChangedCallerRotationValue;
    }

    private void ONChangedCallerRotationValue(Quaternion previousValue, Quaternion newValue)
    {
        transform.rotation = newValue;
    }

    private void ONChangedCallerPositionValue(Vector3 previousValue, Vector3 newValue)
    {
        transform.position += newValue;
    }

    private void ONChangedFillProgressValue(float previousValue, float newValue)
    {
        UpdateProjectors();
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
        ReassignMaterials();
    }

    private void UpdateDecalProjector(DecalProjector decalProjector)
    {
        if (decalProjector == null)
            return;

        Vector3 currentSize;
        currentSize.x = Radius * 2; //_radius는 반지름의 길이 이므로 Project의 크기는 2배로 키워야함
        currentSize.y = Radius * 2;
        currentSize.z = Radius;

        decalProjector.size = currentSize;

        if (decalProjector.material == null)
            return;

        float arcAngleNormalized = 1f - Arc / 360;
        decalProjector.material.SetFloat(ArcShaderID, arcAngleNormalized);
        float normalizedAngle = Mathf.Repeat((Angle - 90) % 360, 360) / 360;
        decalProjector.material.SetFloat(AngleShaderID, normalizedAngle);
        decalProjector.material.SetFloat(FillProgressShaderID, FillProgress);
    }

    private void UpdateDecalFillProgressProjector()
    {
        _decal_Circle_projector.material.SetFloat(FillProgressShaderID, FillProgress);
        _decal_CircleBorder_projector.material.SetFloat(FillProgressShaderID, FillProgress);
    }


    private void ReassignMaterials()
    {
        if (_decal_Circle_projector != null)
            _decal_Circle_projector.material = new Material(_decal_Circle_projector.material);

        if (_decal_CircleBorder_projector != null)
            _decal_CircleBorder_projector.material = new Material(_decal_CircleBorder_projector.material);
    }

    public void UpdateProjectors()
    {
        UpdateDecalProjector(_decal_Circle_projector);
        UpdateDecalProjector(_decal_CircleBorder_projector);
    }

    public void SetValue(float radius, float arc, Transform callerTr, Action anotherOption = null)
    {
        Radius = radius;
        Arc = arc;

        //Vector3 currentSize;
        //currentSize.x = radius * 2; //_radius는 반지름의 길이 이므로 Project의 크기는 2배로 키워야함
        //currentSize.y = radius * 2;
        //currentSize.z = Depth;
        CallerPosition = callerTr.position;
        CallerLocation = callerTr.rotation;
        doneEvent += anotherOption;
        StartCoroutine(Play_Indicator());
    }

    IEnumerator Play_Indicator()
    {
        float _charging = 0f;
        while(FillProgress < 1)
        {
            _charging = Mathf.Clamp01(_charging += Time.deltaTime * 0.45f);
            FillProgress = _charging;
            UpdateDecalFillProgressProjector();
            yield return null;
        }
        doneEvent?.Invoke();
        FillProgress = 0f;
        _charging = 0f;
        Managers.ResourceManager.DestroyObject(gameObject);

    }
    private void OnValidate() => UpdateProjectors();
#if UNITY_EDITOR
    // 에디터에서만 유효한 코드
    private void Reset()
    {
        Bind<DecalProjector>(typeof(DecalProjectors));
        _decal_Circle_projector = Get<DecalProjector>((int)DecalProjectors.Circle);
        _decal_CircleBorder_projector = Get<DecalProjector>((int)DecalProjectors.CircleBorder);
        // Reset 후, 바로 Projector 업데이트
        UpdateProjectors();
    }

#endif
}
