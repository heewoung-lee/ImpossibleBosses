using System.Collections.Generic;
using GameManagers;
using NetWork.BaseNGO;
using UnityEngine;
using Zenject;

namespace NetWork.NGO.UI
{
    public class NgoGamePlaySceneSpawn : NetworkBehaviourBase
    {
        [Inject] private RelayManager _relayManager;

        GameObject _player;
        protected override void AwakeInit()
        { 
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            HostSpawnObject();
            Managers.NgoPoolManager.Create_NGO_Pooling_Object();//네트워크 오브젝트 풀링 생성
        }
        private void HostSpawnObject()
        {
            if (IsHost == false)
                return;
            _relayManager.SpawnToRPC_Caller();
            _relayManager.SpawnNetworkObj("Prefabs/NGO/VFX_Root_NGO");
            RequestSpawnToNpc(new List<(string, Vector3)>() //데미지 테스트용 더미 큐브
            {
                {("Prefabs/NPC/Damage_Test_Dummy",new Vector3(10f,0,-2.5f))}
            });
            _relayManager.SpawnNetworkObj("Prefabs/NGO/Scene_NGO/NGO_BossRoomEntrance",_relayManager.NgoRoot.transform);
            _relayManager.SpawnNetworkObj("Prefabs/NGO/Scene_NGO/NGO_Stage_Timer_Controller", _relayManager.NgoRoot.transform);
        }

        private void RequestSpawnToNpc(List<(string, Vector3)> npcPathAndTr)
        {
            foreach ((string, Vector3) npcdata in npcPathAndTr)
            {
                _relayManager.SpawnNetworkObj($"{npcdata.Item1}", _relayManager.NgoRoot.transform, position: npcdata.Item2);
            }
        }
        protected override void StartInit()
        {
        }
    }
}