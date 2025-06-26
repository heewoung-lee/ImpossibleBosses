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
        private Action<PlayerController> _onPlayerSpawnwithController;
        public event Action<PlayerController> OnPlayerSpawnwithController
        {
            add { UniqueEventRegister.AddSingleEvent(ref _onPlayerSpawnwithController, value); }
            remove { UniqueEventRegister.RemovedEvent(ref _onPlayerSpawnwithController, value); }
        }
        public event Action OnBossSpawnEvent
        {
            add
            {
              UniqueEventRegister.AddSingleEvent(ref _onBossSpawnEvent, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _onBossSpawnEvent, value);
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


        public void InvokePlayerSpawnWithController(PlayerController controller)
        {
            _onPlayerSpawnwithController?.Invoke(controller);
            _onPlayerSpawnwithController = null; // 중복호출을 막기 위해 이벤트를 비움
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