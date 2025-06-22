using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using GameManagers.Interface.Resources_Interface;
using Player;
using Stats;
using UnityEngine;
using Util;
using Zenject;
using Environment = Util.Environment;

namespace GameManagers
{
    public class GameManagerEx : IManagerInitializable
    {
        
        [Inject] IDestroyObject _destroyer;
        [Inject] private IInstantiate _instantiate;
        
        
        private GameObject _player;
        private GameObject _bossMonster;
        private Environment _environment;
        private GameObject _spawnPoint;
        private HashSet<GameObject> _enemy = new HashSet<GameObject>();

        
        public Action<int> SpawnEvent;

        private Action _onBossSpawnEvent;
        public event Action OnBossSpawnEvent
        {
            add
            {
                if (_onBossSpawnEvent != null && _onBossSpawnEvent.GetInvocationList().Contains(value) == true)
                    return;

                _onBossSpawnEvent += value;
            }
            remove
            {
                if (_onBossSpawnEvent == null || _onBossSpawnEvent.GetInvocationList().Contains(value) == false)
                {
                    Debug.LogWarning($"There is no such event to remove. Event Target:{value?.Target}, Method:{value?.Method.Name}");
                    return;
                }
                _onBossSpawnEvent -= value;
            }
        }


        private Action<PlayerStats> _onPlayerSpawnEvent;
        public event Action<PlayerStats> OnPlayerSpawnEvent
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _onPlayerSpawnEvent, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _onPlayerSpawnEvent, value);
            }
        }

        private Action<PlayerController> _onPlayerSpawnwithController;
        public event Action<PlayerController> OnPlayerSpawnwithController
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _onPlayerSpawnwithController, value);                
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _onPlayerSpawnwithController, value);
            }
        }
        public void OnPlayerSpawnWithControllerModule(PlayerController playerController)
        {
            _onPlayerSpawnwithController?.Invoke(playerController);
        }

        public GameObject Player { get => _player; }
        public GameObject BossMonster { get => _bossMonster; }
        public Environment EnvironMent { get => _environment; }
        public GameObject SpawnPoint { get => _spawnPoint; }
        public HashSet<GameObject> Enemy { get => _enemy; }

        public GameObject Spawn(string path, Transform parent = null)
        {
            GameObject go = _instantiate.InstantiateByPath(path, parent);

            switch (GetWorldObjectType(go))
            {
                case Define.WorldObject.Unknown:
                    Debug.Log($"Unkown this object: {go.name}");
                    break;
                case Define.WorldObject.Player:
                    _player = go;
                    break;
                case Define.WorldObject.Monster:
                    _enemy.Add(go);
                    SpawnEvent?.Invoke(1);
                    break;
                case Define.WorldObject.Boss:
                    _bossMonster = go;
                    break;
            }

            return go;
        }

        public void SetPlayer(GameObject playerObject)
        {
            _player = playerObject;
            _onPlayerSpawnEvent?.Invoke(playerObject.GetComponent<PlayerStats>());
        }
        public void SetBossMonster(GameObject bossMonster)
        {
            _bossMonster = bossMonster;
            _onBossSpawnEvent?.Invoke();
        }


        public Define.WorldObject GetWorldObjectType(GameObject go)
        {
            if (go.TryGetComponent(out BaseController baseController))
                return baseController.WorldobjectType;

            return Define.WorldObject.Unknown;
        }

        public void Despawn(GameObject go)
        {
            switch (GetWorldObjectType(go))
            {
                case Define.WorldObject.Unknown:
                    Debug.Log($"Unkown this object: {go.name}");
                    break;
                case Define.WorldObject.Player:
                    _player = null;
                    break;
                case Define.WorldObject.Monster:
                    if (_enemy.Contains(go))
                    {
                        _enemy.Remove(go);
                        SpawnEvent?.Invoke(-1);
                    }
                    break;
            }
            _destroyer.DestroyObject(go);
        }

        public void Init()
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