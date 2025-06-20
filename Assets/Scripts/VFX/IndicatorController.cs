using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GameManagers;
using GameManagers.Interface.Resources_Interface;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Splines.ExtrusionShapes;
using Zenject;

public class IndicatorController : MonoBehaviour, IIndicatorBahaviour
{
    private const float Depth = 100f;
    [Inject] IDestroyObject _destroyer;
    public int ID = 0;

    enum DecalProjectors
    {
        Circle,
        CircleBorder
    }

    private float _radius;
    private float _angle;
    private float _arc;
    private Vector3 _callerPosition;

    private DecalProjector _decalCircleProjector;
    private DecalProjector _decalCircleBorderProjector;

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
            currentSize.z = Depth;

            _decalCircleProjector.size = currentSize;
            _decalCircleBorderProjector.size = currentSize;
        }
    }
    public float Angle
    {
        get => _angle;
        private set
        {
            _angle = value;
            float normalizedAngle = Mathf.Repeat((_angle - 90) % 360, 360) / 360;
            _decalCircleBorderProjector.material.SetFloat(AngleShaderID, normalizedAngle);
            _decalCircleProjector.material.SetFloat(AngleShaderID, normalizedAngle);
        }
    }
    public float Arc
    {
        get => _arc;
        private set
        {
            _arc = Mathf.Clamp(value, 0f, 360f);
            float arcAngleNormalized = 1f - _arc / 360;
            _decalCircleProjector.material.SetFloat(ArcShaderID, arcAngleNormalized);
            _decalCircleBorderProjector.material.SetFloat(ArcShaderID, arcAngleNormalized);
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
        _decalCircleProjector = transform.Find(DecalProjectors.Circle.ToString()).GetComponent<DecalProjector>();
        _decalCircleBorderProjector = transform.Find(DecalProjectors.CircleBorder.ToString()).GetComponent<DecalProjector>();
        GetComponent<Poolable>().WorldPositionStays = false;
        ReassignMaterials();
    }
    private void ReassignMaterials()
    {
        if (_decalCircleProjector != null)
            _decalCircleProjector.material = new Material(_decalCircleProjector.material);

        if (_decalCircleBorderProjector != null)
            _decalCircleBorderProjector.material = new Material(_decalCircleBorderProjector.material);
    }
    private void UpdateDecalFillProgressProjector(float fillAmount)
    {
        _decalCircleProjector.material.SetFloat(FillProgressShaderID, fillAmount);
        _decalCircleBorderProjector.material.SetFloat(FillProgressShaderID, fillAmount);
    }
    public void SetValue(float radius, float arc, Transform targetTr, float duration, Action indicatorDoneEvent = null)
    {
        Radius = radius;
        Arc = arc;
        CallerPosition = targetTr.position;
        Angle = targetTr.eulerAngles.y;
        _doneIndicatorEvent += indicatorDoneEvent;
        StartCoroutine(Play_Indicator(duration));
    }
    public void SetValue(float radius, float arc, Vector3 targetPos,float duration, Action indicatorDoneEvent = null)
    {
        Radius = radius;
        Arc = arc;
        CallerPosition = targetPos;
        _doneIndicatorEvent += indicatorDoneEvent;
        StartCoroutine(Play_Indicator(duration));
    }

    private IEnumerator Play_Indicator(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // 0~1 로 정규화된 진행 비율
            float fillAmount = Mathf.Clamp01(elapsed / duration);
            UpdateDecalFillProgressProjector(fillAmount);
            yield return null;
        }
        _doneIndicatorEvent?.Invoke();
        _doneIndicatorEvent = null;
        UpdateDecalFillProgressProjector(0f);       // 다음 재사용 대비
        _destroyer.DestroyObject(gameObject);
    }
}