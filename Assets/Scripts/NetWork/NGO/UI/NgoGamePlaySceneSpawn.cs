using System.Collections.Generic;
using GameManagers;
using NetWork.BaseNGO;
using NetWork.NGO.InitializeNGO;
using NPC.Dummy;
using UnityEngine;
using Zenject;

namespace NetWork.NGO.UI
{
    public class NgoGamePlaySceneSpawn : NetworkBehaviourBase
    {
        public class NgoGamePlaySceneSpawnFactory : NgoZenjectFactory<NgoGamePlaySceneSpawn>
        {
            public NgoGamePlaySceneSpawnFactory(DiContainer container, GameObject ngo)
            {
                _container = container;
                _ngo = ngo;
            }
        }
        
        [Inject] private RelayManager _relayManager;
        [Inject] private NgoPoolManager _poolManager;
        [Inject] private IFactory<NgoVFXInitalize> _ngoVFXRootFactory;
        [Inject] private IFactory<Dummy> _dummyFactory;
        GameObject _player;
        protected override void AwakeInit()
        { 
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            HostSpawnObject();
            _poolManager.Create_NGO_Pooling_Object();//네트워크 오브젝트 풀링 생성
        }
        private void HostSpawnObject()
        {
            if (IsHost == false)
                return;
            _relayManager.SpawnToRPC_Caller();
            
            NgoVFXInitalize vfxRoot = _ngoVFXRootFactory.Create();
            _relayManager.SpawnNetworkObj(vfxRoot.gameObject);


            Dummy dummy = _dummyFactory.Create();
            _relayManager.SpawnNetworkObj(dummy.gameObject,_relayManager.NgoRoot.transform,position:new Vector3(10f,0,-2.5f));
            
            
            _relayManager.SpawnNetworkObj("Prefabs/NGO/Scene_NGO/NGO_BossRoomEntrance",_relayManager.NgoRoot.transform);
            _relayManager.SpawnNetworkObj("Prefabs/NGO/Scene_NGO/NGO_Stage_Timer_Controller", _relayManager.NgoRoot.transform);
        }

        protected override void StartInit()
        {
        }
    }
}