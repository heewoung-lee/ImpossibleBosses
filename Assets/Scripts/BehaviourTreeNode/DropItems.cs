using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class DropItems : Action
{
    private int minimumTimeCount = 1;
    private int maximumTimeCount = 3;


    private int _spwanItemCount = 10;
    private List<int> _timeRandom;
    private int _index;
    private bool _isCallIndex;
    float _elapseTime = 0;
    BehaviorTree _tree;

    GameObject _ngo_dropItemBehaviour;
    public override void OnStart()
    {
        base.OnStart();
        _tree = Owner.GetComponent<BehaviorTree>();
        _ngo_dropItemBehaviour = Managers.ResourceManager.Instantiate("Prefabs/NGO/NGO_EmptyObject");
        Managers.RelayManager.SpawnNetworkOBJ(_ngo_dropItemBehaviour,Managers.RelayManager.NGO_ROOT.transform);
        _ngo_dropItemBehaviour.AddComponent<DropItemBehaviour>();
        _index = 0;
        _isCallIndex = false;

        _timeRandom = new List<int>();
        for (int i = 0; i < _spwanItemCount; i++)
        {
            int randomNumber = Random.Range(minimumTimeCount, maximumTimeCount);
            _timeRandom.Add(randomNumber);
        }
        
    }


    public override TaskStatus OnUpdate()
    {
        if(_index >= _timeRandom.Count)
        {
            if (_tree != null)
            {
                _tree.DisableBehavior(); // 내부적으로 정리하면서 비활성화
            }
            
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
                NetworkObjectReference dropItemBahaviour = Managers.RelayManager.GetNetworkObject(_ngo_dropItemBehaviour);
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
        Managers.RelayManager.DeSpawn_NetWorkOBJ(_ngo_dropItemBehaviour);
    }
}
