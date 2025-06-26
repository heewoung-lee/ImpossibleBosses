using System;
using System.Collections;
using GameManagers;
using GameManagers.Interface.Resources_Interface;
using GameManagers.Interface.ResourcesManager;
using NetWork.BaseNGO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Util;
using Zenject;

namespace VFX
{
    public class NgoIndicatorController : NetworkBehaviourBase, IIndicatorBahaviour
    {
        [Inject] IDestroyObject _destroyer;
        private const float Depth = 10f;

        enum DecalProjectors
        {
            Circle,
            CircleBorder
        }

        private Action _onIndicatorDone;
        public event Action OnIndicatorDone
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _onIndicatorDone, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _onIndicatorDone, value);
            }
        }

        private NetworkVariable<float> _radius = new NetworkVariable<float>
            (0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private NetworkVariable<float> _angle = new NetworkVariable<float>
            (0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private NetworkVariable<float> _arc = new NetworkVariable<float>
            (0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private NetworkVariable<Vector3> _callerPosition = new NetworkVariable<Vector3>
            (Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private NetworkVariable<float> _durationTime = new NetworkVariable<float>
            (0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private DecalProjector _decalCircleProjector;
        private DecalProjector _decalCircleBorderProjector;


        public float Radius
        {
            get => _radius.Value;

            private set
            {
                if (IsHost == false) return;
                _radius.Value = Mathf.Max(value, 0f);
            }
        }
        public float Angle
        {
            get => _angle.Value;
            private set
            {
                if (IsHost == false) return;
                _angle.Value = value;
            }
        }
        public float Arc
        {
            get => _arc.Value;
            private set
            {
                if (IsHost == false) return;
                _arc.Value = Mathf.Clamp(value, 0f, 360f);
            }
        }
        public Vector3 CallerPosition
        {
            get => _callerPosition.Value;
            private set
            {
                if (IsHost == false) return;
                _callerPosition.Value = value;
            }
        }
        public float DurationTime
        {
            get => _durationTime.Value;
            set
            {
                if (IsHost == false) return;
                _durationTime.Value = value;
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
            _decalCircleProjector.material.SetFloat(ArcShaderID, arcAngleNormalized);
            _decalCircleBorderProjector.material.SetFloat(ArcShaderID, arcAngleNormalized);
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
            currentSize.z = Depth;

            _decalCircleProjector.size = currentSize;
            _decalCircleBorderProjector.size = currentSize;
        }

        private void OnAngleValueChanged(float previousValue, float newValue)
        {
            float normalizedAngle = Mathf.Repeat((newValue - 90) % 360, 360) / 360;
            _decalCircleBorderProjector.material.SetFloat(AngleShaderID, normalizedAngle);
            _decalCircleProjector.material.SetFloat(AngleShaderID, normalizedAngle);
        }
        protected override void StartInit()
        {
        }

        protected override void AwakeInit()
        {
            Bind<DecalProjector>(typeof(DecalProjectors));
            _decalCircleProjector = Get<DecalProjector>((int)DecalProjectors.Circle);
            _decalCircleBorderProjector = Get<DecalProjector>((int)DecalProjectors.CircleBorder);
            GetComponent<Poolable>().WorldPositionStays = false;
            if (TryGetComponent(out NgoPoolingInitalizeBase initbase))
            {
                initbase.PoolObjectReleaseEvent += ReleseProjector;
            }
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
        public void SetValue(float radius, float arc, Transform targetTr,float durationTime,Action indicatorDoneEvent = null)
        {
            Radius = radius;
            Arc = arc;
            CallerPosition = targetTr.position;
            Angle = targetTr.eulerAngles.y;
            OnIndicatorDone += indicatorDoneEvent;
            float clampDuration = Mathf.Max(durationTime, 0.1f);
            DurationTime = clampDuration;
            StartProjectorCoroutineRpc(clampDuration);
        }
        public void SetValue(float radius, float arc, Vector3 targetPos, float durationTime,Action indicatorDoneEvent = null)
        {
            Radius = radius;
            Arc = arc;
            CallerPosition = targetPos;
            float clampDuration = Mathf.Max(durationTime, 0.1f);
            DurationTime = clampDuration;
            StartProjectorCoroutineRpc(clampDuration);
        }

        [Rpc(SendTo.ClientsAndHost)] 
        public void StartProjectorCoroutineRpc(float durationTime)
        {
            StartCoroutine(Play_Indicator(durationTime));
        }
        private IEnumerator Play_Indicator(float duration)
        {

            float elapsed = 0f;
            double nowTime = Managers.RelayManager.NetworkManagerEx.ServerTime.Time;
            while (elapsed < duration)
            {
                double currentNetTime = Managers.RelayManager.NetworkManagerEx.ServerTime.Time;
                double deltaTime = currentNetTime - nowTime;
                nowTime = currentNetTime;

                elapsed += (float)deltaTime;
                // 0~1 로 정규화된 진행 비율
                float fillAmount = Mathf.Clamp01(elapsed / duration);
                UpdateDecalFillProgressProjector(fillAmount);
                yield return null;
            }
            _onIndicatorDone?.Invoke();
            UpdateDecalFillProgressProjector(0f);       // 다음 재사용 대비
            _destroyer.DestroyObject(gameObject);
        }


        private void ReleseProjector()
        {
            CallerPosition = Vector3.zero;
            Radius = 0f;
            Arc = 0f;
            Angle = 0f;
            UpdateDecalFillProgressProjector(0f);
            transform.position = Vector3.zero;
            _onIndicatorDone = null;
        }
    }
}
