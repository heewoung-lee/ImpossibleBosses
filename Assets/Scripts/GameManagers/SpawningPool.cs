using System.Collections;
using GameManagers.Interface.Resources_Interface;
using UnityEngine;
using UnityEngine.AI;
using Util;
using Zenject;

namespace GameManagers
{
    public class SpawningPool : MonoBehaviour
    {
        [Inject] IInstantiate _instantiate;
        [Inject] GameManagerEx _gameManagerEx;
        
        private int _monsterCount = 0;
        private int _reserveMonsterCount = 0;
        private int _keepMonsterCount = 0;
        private Vector3 _spawnPosition = Vector3.zero;
        private float _spawnRange = 15f;
        private float _spawnTime = 5f;

        public void AddCount(int value)
        {
            _monsterCount += value;
        }


        private void Start()
        {
            _gameManagerEx.SpawnEvent -= AddCount;
            _gameManagerEx.SpawnEvent += AddCount;
        }

        public void SetKeepMonsterCount(int keepMonsterCount)
        {
            _keepMonsterCount = keepMonsterCount;
        }

        void Update()
        {
            while (_monsterCount + _reserveMonsterCount < _keepMonsterCount)
            {
                StartCoroutine(SpawnRandomPoint());
            }
        }

        IEnumerator SpawnRandomPoint()
        {
            _reserveMonsterCount++;
            yield return new WaitForSeconds(Random.Range(0,_spawnTime));
            GameObject enemy = _gameManagerEx.Spawn("Prefabs/Enemy/EnemyCube", transform);
            NavMeshAgent nma = _instantiate.GetOrAddComponent<NavMeshAgent>(enemy);
            Vector3 ranpos = Vector3.zero;
            while (true)
            {

                Vector3 ranDir = Random.insideUnitSphere * Random.Range(0, _spawnRange);
                ranpos = ranDir + _spawnPosition;

                NavMeshPath path = new NavMeshPath();

                if (nma.CalculatePath(ranpos, path))
                    break;
            }
            enemy.transform.position = ranpos;
            _reserveMonsterCount--;
        }
    }
}
