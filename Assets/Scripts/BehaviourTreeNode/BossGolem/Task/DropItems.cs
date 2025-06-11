using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Data.DataType.ItemType.Interface;
using GameManagers;
using Unity.Netcode;
using UnityEngine;

namespace BehaviourTreeNode.BossGolem.Task
{
    public class DropItems : Action
    {
        private readonly int _minimumTimeCount = 1;
        private readonly int _maximumTimeCount = 3;


        [SerializeField] private int _spwanItemCount;
        private List<int> _timeRandom;
        private int _index;
        private bool _isCallIndex;
        float _elapseTime = 0;
        BehaviorTree _tree;

        GameObject _ngoDropItemBehaviour;
        public override void OnStart()
        {
            base.OnStart();
            _tree = Owner.GetComponent<BehaviorTree>();
            _ngoDropItemBehaviour = Managers.ResourceManager.Instantiate("Prefabs/NGO/NGO_BossDropItemBehaviour");
            Managers.RelayManager.SpawnNetworkOBJ(_ngoDropItemBehaviour,Managers.RelayManager.NGO_ROOT.transform);
            _index = 0;
            _isCallIndex = false;

            _timeRandom = new List<int>();
            for (int i = 0; i < _spwanItemCount; i++)
            {
                int randomNumber = Random.Range(_minimumTimeCount, _maximumTimeCount);
                _timeRandom.Add(randomNumber);
            }
        
        }


        public override TaskStatus OnUpdate()
        {
            if(_index >= _timeRandom.Count)
            {
                return TaskStatus.Success;
            }

            if (_elapseTime >= _timeRandom[_index] && _isCallIndex == false)
            {
                _isCallIndex = true;
                _elapseTime = 0;
                _index++;
                SpawnItem();

                void SpawnItem()
                {
                    if (Managers.RelayManager.NetworkManagerEx.IsHost == false)
                        return;

                    IItem spawnItem = Managers.ItemDataManager.GetRandomItemFromAll();
                    IteminfoStruct itemStruct = new IteminfoStruct(spawnItem);
                    NetworkObjectReference dropItemBahaviour = Managers.RelayManager.GetNetworkObject(_ngoDropItemBehaviour);
                    Managers.RelayManager.NGO_RPC_Caller.Spawn_Loot_ItemRpc(itemStruct, Owner.transform.position, addLootItemBehaviour:dropItemBahaviour);
                }
            }
            _isCallIndex = false;
            _elapseTime += Time.deltaTime;
            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            base.OnEnd();
            _elapseTime = 0;
            if (_timeRandom != null)
            {
                _timeRandom.Clear();
                _timeRandom = null;
            }
            Managers.RelayManager.DeSpawn_NetWorkOBJ(_ngoDropItemBehaviour);
        }
    }
}
