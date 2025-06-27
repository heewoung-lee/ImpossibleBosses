using GameManagers.Interface.Resources_Interface;
using GameManagers.Interface.ResourcesManager;
using UnityEngine;
using Util;
using Zenject;

namespace GameManagers
{
    //TODO: 스폰포인트 사용법 다듬을 것, 현재는 단순히 초기화 지정만 하는데, 이후에는 원하는 스폰장소를 뽑아올 수 있도록 수정
    public class SpawnManager : IInitializable
    {
        [Inject] private IInstantiate _instantiate;
        
        private GameObject _spawnPoint;
        private Environment _environment;

        public void Initialize()
        {
            _spawnPoint = new GameObject() { name = "@SpawnPoint" };
            _environment = GameObject.FindAnyObjectByType<Environment>();

            if (_environment == null)
            {
                GameObject environmentGo = new GameObject() { name = "@Environment" };
                _environment = _instantiate.GetOrAddComponent<Environment>(environmentGo);
            }

            foreach (Transform childTr in _environment.transform)
            {
                if (childTr.gameObject.TryGetComponentInsChildren(out SpawnPoint[] spawnPointParent))
                {
                    foreach (SpawnPoint spawnPoint in spawnPointParent)
                    {
                        spawnPoint.transform.SetParent(_spawnPoint.transform);
                    }
                }
            }
        }
    }
}
