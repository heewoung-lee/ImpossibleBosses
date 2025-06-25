using System;
using Player;
using Stats;
using UnityEngine;

namespace GameManagers.Interface.GameManagerEx
{
    public interface IPlayerSpawnManager
    {
        public void SetPlayer(GameObject player);
        public GameObject GetPlayer();
        public event Action<PlayerStats> OnPlayerSpawnEvent;
    }
}
