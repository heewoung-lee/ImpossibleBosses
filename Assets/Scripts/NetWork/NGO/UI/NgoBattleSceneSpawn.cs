using System.Collections.Generic;
using GameManagers;
using NetWork.BaseNGO;
using UnityEngine;
using Zenject;

namespace NetWork.NGO.UI
{
    public class NgoBattleSceneSpawn : NetworkBehaviourBase
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
        }
        private void HostSpawnObject()
        {
            if (IsHost == false)
                return;
            _relayManager.SpawnToRPC_Caller();
            _relayManager.SpawnNetworkObj("Prefabs/NGO/VFX_Root_NGO");
            RequestSpawnToNpc(new List<(string, Vector3)>
            {
                ("Prefabs/Enemy/Boss/Character/StoneGolem",new Vector3(10f,0f,10f))
            });
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