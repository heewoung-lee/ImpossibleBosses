using System;
using System.Collections;
using BehaviorDesigner.Runtime;
using Controller;
using Controller.BossState;
using GameManagers;
using GameManagers.Interface.GameManagerEx;
using NetWork.BaseNGO;
using Unity.Netcode;
using UnityEngine;
using VFX;
using Zenject;

namespace NetWork.Boss_NGO
{
    public struct CurrentAnimInfo : INetworkSerializable
    {

        public float AnimLength;
        public float DecelerationRatio;
        public float AnimStopThreshold;
        public float AddIndicatorDuration;
        public double ServerTime;
        public float StartAnimationSpeed;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref AnimLength);
            serializer.SerializeValue(ref DecelerationRatio);
            serializer.SerializeValue(ref AnimStopThreshold);
            serializer.SerializeValue(ref AddIndicatorDuration);
            serializer.SerializeValue(ref ServerTime);
            serializer.SerializeValue(ref StartAnimationSpeed);
        }

        public CurrentAnimInfo(float animLength, float decelerationRatio, float animStopThreshold,float AddindicatorDuration,double serverTime,float startAnimSpeed = 1f)
        {
            AnimLength = animLength;
            DecelerationRatio = decelerationRatio;
            AnimStopThreshold = animStopThreshold;
            AddIndicatorDuration = AddindicatorDuration;
            ServerTime = serverTime;
            StartAnimationSpeed = startAnimSpeed;
        }
    }


    public class BossGolemNetworkController : NetworkBehaviourBase
    {
        [Inject] IBossSpawnManager _bossSpawnManager;
        [Inject] private RelayManager _relayManager;
        
        private readonly float _normalAnimSpeed = 1f;
        private readonly float _maxAnimSpeed = 3f;


        private BehaviorTree _bossBehaviourTree;
        private BossGolemController _bossController;
        private bool _finishedAttack = false;
        private Coroutine _animationCoroutine;
        private bool _finishedIndicatorDuration = false;

        public bool FinishAttack
        {
            get => _finishedAttack;
            private set => _finishedAttack = value;
        }
        protected override void AwakeInit()
        {
            _bossController = GetComponent<BossGolemController>();
            _bossBehaviourTree = GetComponent<BehaviorTree>();
        }
        protected override void StartInit()
        {
        }


        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _bossSpawnManager.SetBossMonster(gameObject);
            if (IsHost == false)
            {
                InitBossOnClient();
            }
            void InitBossOnClient()
            {
                GetComponent<BossController>().enabled = false;
                GetComponent<BehaviorTree>().enabled = false;
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void StartAnimChagnedRpc(CurrentAnimInfo animinfo,NetworkObjectReference indicatorRef = default)
        {
            NgoIndicatorController indicatorController = null;
            if (indicatorRef.Equals(default) == false)
            {
                if (indicatorRef.TryGet(out NetworkObject ngo))
                {
                    indicatorController = ngo.GetComponent<NgoIndicatorController>();
                }
            }
       

            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);

            if (animinfo.AddIndicatorDuration < 0f)        // faster swing
            {
                float speedScale = animinfo.AnimLength / Mathf.Max(animinfo.AnimLength + animinfo.AddIndicatorDuration, 0.1f);
                animinfo.StartAnimationSpeed *= speedScale;
                animinfo.AnimStopThreshold *= speedScale;
            }
            _bossController.Anim.speed = animinfo.StartAnimationSpeed;
            _animationCoroutine = StartCoroutine(UpdateAnimCorutine(animinfo, indicatorController));
        }


        IEnumerator UpdateAnimCorutine(CurrentAnimInfo animinfo, NgoIndicatorController indicatorCon = null)
        {

            double elaspedTime = 0f;
            FinishAttack = false;
            _finishedIndicatorDuration = false;
            double nowTime = _relayManager.NetworkManagerEx.ServerTime.Time;

        
            //현재 서버가 간 시간
            double serverPreTime =  animinfo.ServerTime- nowTime;

            //애니메이션 길이 X 애니메이션이 줄어들어야할 지점
            double decelerationEndTime = animinfo.AnimLength * animinfo.DecelerationRatio;

            //클라이언트가 i아가야할 애니메이션길이
            double remainingAnimTime = decelerationEndTime - serverPreTime;

            //클라이언트가 i아가기 위해서 호스트보다 얼만큼 애니메이션이 빨라야 하는지
            double catchAnimSpeed = Math.Clamp(decelerationEndTime/ remainingAnimTime, _normalAnimSpeed, _maxAnimSpeed);

            if (indicatorCon != null)
            {
                indicatorCon.OnIndicatorDone += () => { _finishedIndicatorDuration = true; };
            }
            else
            {
                StartCoroutine(UpdateIndicatorDurationTime(animinfo.AddIndicatorDuration, animinfo.AnimLength, nowTime));
            }
            while (elaspedTime <=animinfo.AnimLength)
            {
                double currentNetTime = _relayManager.NetworkManagerEx.ServerTime.Time;
                double deltaTime = (currentNetTime - nowTime);
                nowTime = currentNetTime;

                //여기서 현재 인디케이터가 다 마쳤는지 확인해야함
                if (_bossController.TryGetAnimationSpeed(elaspedTime, out float animspeed, animinfo, _finishedIndicatorDuration) == false)
                {
                    elaspedTime += deltaTime * animspeed * catchAnimSpeed;
                }
                else
                {
                    elaspedTime += deltaTime * animspeed;
                }
                yield return null;
            }
            FinishAttack = true;
            _bossController.Anim.speed = 1;
        }


        IEnumerator UpdateIndicatorDurationTime(float indicatorAddduration, float animLength, double prevNetTime)
        {
            _finishedIndicatorDuration = false;
            float elapsedTime = 0f;
            while (elapsedTime <= indicatorAddduration + animLength)
            {
                double currentNetTime = _relayManager.NetworkManagerEx.ServerTime.Time;
                float deltaTime = (float)(currentNetTime - prevNetTime);
                elapsedTime += deltaTime;
                yield return null;
            }
            _finishedIndicatorDuration = true;
        }
    }
}