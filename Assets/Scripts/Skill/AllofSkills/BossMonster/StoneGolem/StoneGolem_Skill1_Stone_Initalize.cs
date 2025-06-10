using System.Collections;
using UnityEngine;
using static Define;
using static UnityEngine.UI.GridLayoutGroup;

public class StoneGolem_Skill1_Stone_Initalize : Poolable, ISpawnBehavior
{
    private const float MAX_HEIGHT = 3f;
    private const int FLIGHTDURATION_TIME = 1;
    
    public void SpawnObjectToLocal(in SpawnParamBase stoneParams, string runtimePath = null)
    {
        Collider _bossTr = Managers.GameManagerEx.BossMonster.transform.GetComponent<Collider>();
        StoneGolem_Skill1_Stone_Initalize stone = Managers.ResourceManager.Instantiate(runtimePath).GetComponent<StoneGolem_Skill1_Stone_Initalize>();
        stone.transform.SetParent(Managers.VFX_Manager.VFX_Root, false);
        stone.transform.position = _bossTr.transform.position + Vector3.up * _bossTr.GetComponent<Collider>().bounds.max.y;
        stone.transform.rotation = Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));
        Vector3 targetPos = stoneParams.argPosVector3;
        stone.StartCoroutine(stone.ThrowStoneParabola(stone.transform,targetPos,FLIGHTDURATION_TIME));
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
            float currentY = Mathf.Lerp(startPoint.y, targetPoint.y, t) +MAX_HEIGHT * Mathf.Sin(Mathf.PI * t);

            // 최종 위치 설정
            projectile.position = new Vector3(currentXZ.x, currentY, currentXZ.z);

            yield return null;
        }
        // 포물선 이동 완료 후 파괴
        Managers.ResourceManager.DestroyObject(projectile.gameObject, 2f);
    }
}
