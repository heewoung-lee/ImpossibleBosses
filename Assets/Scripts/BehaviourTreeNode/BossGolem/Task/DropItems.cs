using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Data.DataType.ItemType.Interface;
using GameManagers;
using GameManagers.Interface.ItemDataManager;
using GameManagers.Interface.Resources_Interface;
using GameManagers.Interface.ResourcesManager;
using UI.SubItem;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace BehaviourTreeNode.BossGolem.Task
{
    public class DropItems : Action
    {
        [Inject] private IInstantiate _instantiate;
        [Inject] private IItemGetter _itemGetter;
        [Inject] private RelayManager _relayManager;

        
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
            _ngoDropItemBehaviour = _instantiate.InstantiateByPath("Prefabs/NGO/NGO_BossDropItemBehaviour");
            _relayManager.SpawnNetworkObj(_ngoDropItemBehaviour,_relayManager.NgoRoot.transform);
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
                    if (_relayManager.NetworkManagerEx.IsHost == false)
                        return;

                    IItem spawnItem = _itemGetter.GetRandomItemFromAll();
                    IteminfoStruct itemStruct = new IteminfoStruct(spawnItem);
                    NetworkObjectReference dropItemBahaviour = _relayManager.GetNetworkObject(_ngoDropItemBehaviour);
                    _relayManager.NgoRPCCaller.Spawn_Loot_ItemRpc(itemStruct, Owner.transform.position, addLootItemBehaviour:dropItemBahaviour);
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
            _relayManager.DeSpawn_NetWorkOBJ(_ngoDropItemBehaviour);
        }
    }
}
