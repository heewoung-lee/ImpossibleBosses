using System.Collections.Generic;
using NetWork.NGO;
using Unity.Netcode;
using UnityEngine;
using Util;
using Zenject;

namespace ZenjectTool
{
    public class NgoInjector
    {
        private readonly DiContainer _container;
        private readonly List<GameObject> _ngoPrefabs;
        
        public NgoInjector(DiContainer container, [Inject(Id = Define.ZenjectNgo)] List<GameObject> ngoPrefabs)
        {
            _container = container;
            _ngoPrefabs = ngoPrefabs;
        }
        
        public void NgoRegisterHandlers(NetworkManager networkManager)
        {
            if (networkManager == null)
                return;

            foreach (GameObject prefab in _ngoPrefabs)
            {
                networkManager.PrefabHandler.AddHandler(prefab, new NgoZenjectHandler(_container, prefab));
                Debug.Log($"'{prefab.name}' 핸들러 등록 완료.");
            }
        }
    }   
}
