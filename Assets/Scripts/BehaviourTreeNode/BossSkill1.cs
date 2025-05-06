using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkill1 : Action
{
    private const float MAX_HEIGHT = 3f;
    private const float START_SKILL1_ANIM_SPEED = 0.8f;

    public SharedInt Damage;

    private BossGolemController _controller;
    private BossStats _stats;

    private float _elapsedTime = 0f;
    private int _tickCounter = 0;
    private float _animLength = 0f;

    private bool _isAttackReady = false;
    private Collider[] allTargets;

    private float _attackDelayTime = 1f;

    public override void OnStart()
    {
        base.OnStart();
        ChechedField();

        void ChechedField()
        {
            _controller = Owner.GetComponent<BossGolemController>();
            _stats = _controller.GetComponent<BossStats>();
            _animLength = Utill.GetAnimationLength("Anim_Hit", _controller.Anim);
            allTargets = Physics.OverlapSphere(Owner.transform.position, float.MaxValue, _stats.TarGetLayer);
            _controller.CurrentStateType = _controller.BossSkill1State;
        }
    }

    public override TaskStatus OnUpdate()
    {
        float elaspedTime = UpdateElapsedTime();
        _tickCounter++;
        if (_tickCounter >= 20)
        {
            _tickCounter = 0;
            foreach (Collider targetPlayer in allTargets)
            {
                string skill1_indicator_Path = "Prefabs/Enemy/Boss/Indicator/Boss_Skill1_Indicator";
                Vector3 targetPos = targetPlayer.transform.position;
                Vector3 indicatorAngle = transform.eulerAngles;
                SpawnParamBase param = SpawnParamBase.Create(argPosVector3: targetPos, argEulerAnglesVector3: indicatorAngle,argInteger: Damage.Value);
                Managers.RelayManager.NGO_RPC_Caller.SpawnObjectToLocal(targetPos, skill1_indicator_Path, param);
                //5.6 수정 SpawnProjector(targetPlayer);
            }
        }
        _isAttackReady = _controller.SetAnimationSpeed(_elapsedTime, _animLength, _controller.BossSkill1State,out float animSpeed, START_SKILL1_ANIM_SPEED);

        return _isAttackReady == true ? TaskStatus.Success : TaskStatus.Running;
        float UpdateElapsedTime()
        {
            _elapsedTime += Time.deltaTime * _controller.Anim.speed;
            return _elapsedTime;
        }
    }

    private void SpawnStone(Collider targetPlayer)
    {
        GameObject stone = Managers.ResourceManager.Instantiate("Prefabs/Enemy/Boss/AttackPattren/BossSkill1");
        stone.transform.SetParent(Managers.VFX_Manager.VFX_Root_NGO, false);
        stone.transform.position = Owner.transform.position + Vector3.up * Owner.GetComponent<Collider>().bounds.max.y;
        stone.transform.rotation = Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));
        StartCoroutine(ThrowStoneParabola(stone.transform, targetPlayer, _attackDelayTime));
    }

    private IEnumerator ThrowStoneParabola(Transform projectile, Collider targetPlayer, float duration)
    {
        Vector3 startPoint = projectile.transform.position;
        Vector3 targetPoint = targetPlayer.transform.position;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // t: 진행 비율 (0~1)
            float t = elapsedTime / duration;

            // XZ 위치 보간
            Vector3 currentXZ = Vector3.Lerp(startPoint, targetPoint, t);

            // Y 값은 포물선 계산
            float currentY = Mathf.Lerp(startPoint.y, targetPoint.y, t) +
                             MAX_HEIGHT * Mathf.Sin(Mathf.PI * t);

            // 최종 위치 설정
            projectile.position = new Vector3(currentXZ.x, currentY, currentXZ.z);

            yield return null;
        }
        // 포물선 이동 완료 후 파괴
        Managers.ResourceManager.DestroyObject(projectile.gameObject, 2f);
    }



    public override void OnEnd()
    {
        base.OnEnd();
        _elapsedTime = 0;
        _controller.Anim.speed = 1f;
    }
}