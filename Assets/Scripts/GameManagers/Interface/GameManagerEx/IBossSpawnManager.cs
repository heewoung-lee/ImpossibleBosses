using System;
using Stats;
using UnityEngine;

namespace GameManagers.Interface.GameManagerEx
{
    public interface IBossSpawnManager
    {
        public GameObject GetBossMonster();
        public void SetBossMonster(GameObject bossMonster);
        public event Action OnBossSpawnEvent;
    }
}


