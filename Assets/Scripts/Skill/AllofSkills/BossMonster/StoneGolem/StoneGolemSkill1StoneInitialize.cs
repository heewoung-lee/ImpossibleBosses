using System.Collections;
using GameManagers;
using GameManagers.Interface.GameManagerEx;
using GameManagers.Interface.Resources_Interface;
using NetWork;
using NetWork.NGO.Interface;
using UnityEngine;
using Zenject;

namespace Skill.AllofSkills.BossMonster.StoneGolem
{
    public class StoneGolemSkill1StoneInitialize : Poolable, ISpawnBehavior
    {
        
        [Inject] private IInstantiate _instantiate;
        [Inject] IDestroyObject _destroyer;
        [Inject] IBossSpawnManager _bossSpawnManager;
        
        private const float MaxHeight = 3f;
        private const int FlightdurationTime = 1;
    
        public void SpawnObjectToLocal(in SpawnParamBase stoneParams, string runtimePath = null)
        {
            Collider bossTr = _bossSpawnManager.GetBossMonster().transform.GetComponent<Collider>();
            StoneGolemSkill1StoneInitialize stone = _instantiate.InstantiateByPath(runtimePath).GetComponent<StoneGolemSkill1StoneInitialize>();
            stone.transform.SetParent(Managers.VFXManager.VFXRoot, false);
            stone.transform.position = bossTr.transform.position + Vector3.up * bossTr.GetComponent<Collider>().bounds.max.y;
            stone.transform.rotation = Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));
            Vector3 targetPos = stoneParams.ArgPosVector3;
            stone.StartCoroutine(stone.ThrowStoneParabola(stone.transform,targetPos,FlightdurationTime));
        }
        public IEnumerator ThrowStoneParabola(Transform projectile, Vector3 targetPlayer, float duration)
        {
            Vector3 startPoint = projectile.transform.position;
            Vector3 targetPoint = targetPlayer;
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                // t: 진행 비율 (0~1)
                float t = elapsedTime / duration;

                // XZ 위치 보간
                Vector3 currentXZ = Vector3.Lerp(startPoint, targetPoint, t);

                // Y 값은 포물선 계산
                float currentY = Mathf.Lerp(startPoint.y, targetPoint.y, t) +MaxHeight * Mathf.Sin(Mathf.PI * t);

                // 최종 위치 설정
                projectile.position = new Vector3(currentXZ.x, currentY, currentXZ.z);

                yield return null;
            }
            // 포물선 이동 완료 후 파괴
            _destroyer.DestroyObject(projectile.gameObject, 2f);
        }
    }
}
