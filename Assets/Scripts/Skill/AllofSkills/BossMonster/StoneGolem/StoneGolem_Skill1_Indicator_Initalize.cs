using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class StoneGolem_Skill1_Indicator_Initalize : Poolable, ISpawnBehavior
{
    private const float SKILL1_RADIUS = 2f;
    private const float SKILL2_ARC = 360f;
    private const float AttackDelayTime = 1f;

    private Indicator_Controller _projector;
    private int _attackDamage;

    public void SpawnObjectToLocal(in SpawnParamBase Spawnparam, string runtimePath = null)
    {
        _projector = Managers.ResourceManager.Instantiate(runtimePath).GetComponent<Indicator_Controller>();
        _projector.SetValue(SKILL1_RADIUS, SKILL2_ARC, Spawnparam.argPosVector3,Spawnparam.argEulerAnglesVector3);

        //projector.FillProgress = 0;
        _attackDamage = Spawnparam.argInteger;
      
    }

    private void Start()
    {
        StartCoroutine(startProjector(_projector, _attackDamage));
    }


    private IEnumerator startProjector(Indicator_Controller projector,int damage)
    {
        float elaspedTime = 0f;
        while (elaspedTime < AttackDelayTime)
        {
            elaspedTime += Time.deltaTime;
            float fillRatio = Mathf.Clamp01(elaspedTime / AttackDelayTime);

            // 인디케이터 채우기
            //projector.FillProgress = fillRatio;
            //projector.UpdateProjectors();

            yield return null;
        }
        TargetInSight.AttackTargetInCircle(projector.GetComponent<ProjectorAttack>(), projector.Radius, damage);
    }

}
