using UnityEngine;

namespace GameManagers
{
    public class LootItemManager
    {
        private GameObject _itemRoot;

        public Transform ItemRoot
        {
            get
            {
                if(_itemRoot == null)
                {
                    _itemRoot = Managers.RelayManager.SpawnNetworkObj("Prefabs/NGO/ItemRootNetwork");
                }
                return _itemRoot.transform;
            }
        }
    }
}