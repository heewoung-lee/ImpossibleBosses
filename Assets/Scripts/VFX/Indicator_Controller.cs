using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Splines.ExtrusionShapes;

public class Indicator_Controller : MonoBehaviour, IIndicatorBahaviour
{
    private const float DEPTH = 10f;

    public int ID = 0;

    enum DecalProjectors
    {
        Circle,
        CircleBorder
    }

    [SerializeField]private float _radius;
    [SerializeField] private float _angle;
    [SerializeField] private float _arc;
    [SerializeField] private Vector3 _callerPosition;

    private DecalProjector _decal_Circle_projector;
    private DecalProjector _decal_CircleBorder_projector;

    private Action _doneIndicatorEvent;

    public float Radius
    {
        get => _radius;

        private set
        {
            _radius = Mathf.Max(value, 0f);
            Vector3 currentSize;
            currentSize.x = _radius * 2; //_radius는 반지름의 길이 이므로 Project의 크기는 2배로 키워야함
            currentSize.y = _radius * 2;
            currentSize.z = DEPTH;

            _decal_Circle_projector.size = currentSize;
            _decal_CircleBorder_projector.size = currentSize;
        }
    }
    public float Angle
    {
        get => _angle;
        private set
        {
            _angle = value;
            float normalizedAngle = Mathf.Repeat((_angle - 90) % 360, 360) / 360;
            _decal_CircleBorder_projector.material.SetFloat(AngleShaderID, normalizedAngle);
            _decal_Circle_projector.material.SetFloat(AngleShaderID, normalizedAngle);
        }
    }
    public float Arc
    {
        get => _arc;
        private set
        {
            _arc = Mathf.Clamp(value, 0f, 360f);
            float arcAngleNormalized = 1f - _arc / 360;
            _decal_Circle_projector.material.SetFloat(ArcShaderID, arcAngleNormalized);
            _decal_CircleBorder_projector.material.SetFloat(ArcShaderID, arcAngleNormalized);
        }
    }
    public Vector3 CallerPosition
    {
        get => _callerPosition;
        private set
        {
            _callerPosition = value;
            transform.position = _callerPosition;
        }
    }

    private static readonly int ColorShaderID = Shader.PropertyToID("_Color");

    private static readonly int FillColorShaderID = Shader.PropertyToID("_FillColor");

    private static readonly int FillProgressShaderID = Shader.PropertyToID("_FillProgress");

    private static readonly int ArcShaderID = Shader.PropertyToID("_Arc");

    private static readonly int AngleShaderID = Shader.PropertyToID("_Angle");

    protected void Awake()
    {
        _decal_Circle_projector = transform.Find(DecalProjectors.Circle.ToString()).GetComponent<DecalProjector>();
        _decal_CircleBorder_projector = transform.Find(DecalProjectors.CircleBorder.ToString()).GetComponent<DecalProjector>();
        GetComponent<Poolable>().WorldPositionStays = false;
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
    public void SetValue(float radius, float arc, Transform targetTr, Action indicatorDoneEvent = null)
    {
        Radius = radius;
        Arc = arc;
        CallerPosition = targetTr.position;
        Angle = targetTr.eulerAngles.y;
        _doneIndicatorEvent += indicatorDoneEvent;
        StartCoroutine(Play_Indicator());
    }
    public void SetValue(float radius, float arc, Vector3 targetPos, Action indicatorDoneEvent = null)
    {
        Radius = radius;
        Arc = arc;
        CallerPosition = targetPos;
        _doneIndicatorEvent += indicatorDoneEvent;
        StartCoroutine(Play_Indicator());
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
        _doneIndicatorEvent?.Invoke();
        _doneIndicatorEvent = null;
        UpdateDecalFillProgressProjector(0f);
        Managers.ResourceManager.DestroyObject(gameObject);
    }

}