using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SocialPlatforms;

public class Indicator_Controller : NetworkBehaviour
{
    [SerializeField]
    private NetworkVariable<float> _radius = new NetworkVariable<float>
        (1.0f,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    [SerializeField]
    private NetworkVariable<float> _angle = new NetworkVariable<float>
        (0f,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField]
    private NetworkVariable<float> _arc = new NetworkVariable<float>
        (30f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField]
    private NetworkVariable<float> _fillProgress = new NetworkVariable<float>
        (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField]
    private NetworkVariable<float> _depth = new NetworkVariable<float>
        (10f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    private DecalProjector _decal_Circle_projector;
    private DecalProjector _decal_CircleBorder_projector;
    private GameObject _circle;
    private GameObject _circleBoader;

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

    public float Depth {
        get => _depth.Value;
        set
        {
            if (IsHost == false) return;
            _depth.Value = value;
        }
    }

    private static readonly int ColorShaderID = Shader.PropertyToID("_Color");

    private static readonly int FillColorShaderID = Shader.PropertyToID("_FillColor");

    private static readonly int FillProgressShaderID = Shader.PropertyToID("_FillProgress");

    private static readonly int ArcShaderID = Shader.PropertyToID("_Arc");

    private static readonly int AngleShaderID = Shader.PropertyToID("_Angle");

    private void Awake()
    {
        _circle = transform.Find("Circle").gameObject;
        _decal_Circle_projector = _circle.GetComponent<DecalProjector>();
         _circleBoader = transform.Find("CircleBorder").gameObject;
        _decal_CircleBorder_projector = _circleBoader.GetComponent<DecalProjector>();
        ReassignMaterials();
    }

    private void UpdateDecalProjector(DecalProjector decalProjector)
    {
        if (decalProjector == null)
            return;
        
        Vector3 currentSize;
        currentSize.x = _radius.Value*2; //_radius는 반지름의 길이 이므로 Project의 크기는 2배로 키워야함
        currentSize.y = _radius.Value *2;
        currentSize.z = Depth;

        decalProjector.size = currentSize;

        if (decalProjector.material == null)
            return;

        float arcAngleNormalized = 1f - Arc / 360;
        decalProjector.material.SetFloat(ArcShaderID, arcAngleNormalized);
        float normalizedAngle = Mathf.Repeat((_angle.Value - 90) % 360, 360) / 360;
        decalProjector.material.SetFloat(AngleShaderID, normalizedAngle);
        decalProjector.material.SetFloat(FillProgressShaderID, _fillProgress.Value);
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

    public void SetValue(float radius, float arc)
    {
        Radius = radius;
        Arc = arc;
    }
    private void OnValidate() => UpdateProjectors();
#if UNITY_EDITOR
    // 에디터에서만 유효한 코드
    private void Reset()
    {

        // 오브젝트 참조 초기화 (원하는 방식으로 초기화)
        _circle = transform.Find("Circle").gameObject;
        _decal_Circle_projector = _circle.GetComponent<DecalProjector>();
        _circleBoader = transform.Find("CircleBorder").gameObject;
        _decal_CircleBorder_projector = _circleBoader.GetComponent<DecalProjector>();

        if (_circle)
            _decal_Circle_projector = _circle.GetOrAddComponent<DecalProjector>();
        if (_circleBoader)
            _decal_CircleBorder_projector = _circleBoader.GetOrAddComponent<DecalProjector>();

        // Reset 후, 바로 Projector 업데이트
        UpdateProjectors();
    }
#endif
}
