using UnityEngine;
using Zenject;

namespace GameManagers
{
    public class LootItemManager
    {
        private GameObject _itemRoot;
        [Inject] private RelayManager _relayManager;

        public Transform ItemRoot
        {
            get
            {
                if(_itemRoot == null)
                {
                    _itemRoot = _relayManager.SpawnNetworkObj("Prefabs/NGO/ItemRootNetwork");
                }
                return _itemRoot.transform;
            }
        }
    }
}