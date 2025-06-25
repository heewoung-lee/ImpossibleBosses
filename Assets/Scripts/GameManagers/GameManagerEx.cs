using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using GameManagers.Interface.GameManagerEx;
using GameManagers.Interface.Resources_Interface;
using Player;
using Stats;
using UnityEngine;
using Util;
using Zenject;
using Environment = Util.Environment;

namespace GameManagers
{
    public class GameManagerEx: IPlayerSpawnManager,IBossSpawnManager
    {
        private GameObject _player;
        private GameObject _bossMonster;
        private Action _onBossSpawnEvent;
        private Action<PlayerStats> _onPlayerSpawnEvent;
        
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
                    Debug.LogWarning(
                        $"There is no such event to remove. Event Target:{value?.Target}, Method:{value?.Method.Name}");
                    return;
                }

                _onBossSpawnEvent -= value;
            }
        }

        public GameObject GetPlayer()
        {
            return _player;
        }
        public GameObject GetBossMonster()
        {
            return _bossMonster;
        }
        public event Action<PlayerStats> OnPlayerSpawnEvent
        {
            add { UniqueEventRegister.AddSingleEvent(ref _onPlayerSpawnEvent, value); }
            remove { UniqueEventRegister.RemovedEvent(ref _onPlayerSpawnEvent, value); }
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
    }
}