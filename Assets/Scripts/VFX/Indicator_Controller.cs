using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SocialPlatforms;

public class Indicator_Controller : MonoBehaviour
{

    DecalProjector _decal_Circle_projector;
    DecalProjector _decal_CircleBorder_projector;
    GameObject _circle;
    GameObject _circleBoader;

    [Header("Properties")]
    /// <summary>
    /// The length of the arc.
    /// </summary>
    [SerializeField]
    [Min(0f)]
    private float _radius = 1.0f;

    /// <summary>
    /// The angle offset for the cone in euler angles.
    /// </summary>
    [SerializeField]
    private float _angle;

    /// <summary>
    /// The arc in euler angles. This is the amount that it's opened, 0 being closed and 360 being fully open.
    /// </summary>
    [SerializeField]
    [Range(0, 360)]
    private float _arc = 30.0f;

    /// <summary>
    /// The progress of the fill.
    /// </summary>
    [SerializeField]
    [Range(0, 1)]
    private float _fillProgress;

    /// <summary>
    /// Depth to project the object.
    /// </summary>
    [SerializeField]
    [Min(0)]
    private float _depth = 10;


    public float Radius
    {
        get => _radius;
        set => _radius = Mathf.Max(value, 0f);
    }
    public float Angle
    {
        get => _angle;
        set => _angle = value;
    }
    public float Arc
    {
        get => _arc;
        set => _arc = Mathf.Clamp(value, 0f, 360f);
    }
    public float FillProgress
    {
        get => _fillProgress;
        set
        {
            _fillProgress = value;
            _decal_Circle_projector.material.SetFloat(FillProgressShaderID, _fillProgress);
        }
    }

    public float Depth { 
        get => _depth; 
        set => _depth = value; 
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
        currentSize.x = _radius*2; //_radius는 반지름의 길이 이므로 Project의 크기는 2배로 키워야함
        currentSize.y = _radius*2;
        currentSize.z = Depth;

        decalProjector.size = currentSize;

        if (decalProjector.material == null)
            return;

        float arcAngleNormalized = 1f - Arc / 360;
        decalProjector.material.SetFloat(ArcShaderID, arcAngleNormalized);
        float normalizedAngle = Mathf.Repeat((_angle - 90) % 360, 360) / 360;
        decalProjector.material.SetFloat(AngleShaderID, normalizedAngle);
        decalProjector.material.SetFloat(FillProgressShaderID, _fillProgress);
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
