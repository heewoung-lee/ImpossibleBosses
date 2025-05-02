using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
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
        _controller = Owner.GetComponent<BossGolemController>();
        _stats = _controller.GetComponent<BossStats>();
        _animLength = Utill.GetAnimationLength("Anim_Hit", _controller.Anim);
        allTargets = Physics.OverlapSphere(Owner.transform.position, float.MaxValue, _stats.TarGetLayer);

        _controller.CurrentStateType = _controller.BossSkill1State;
    }

    public override TaskStatus OnUpdate()
    {
        _elapsedTime += Time.deltaTime * _controller.Anim.speed;
        _tickCounter++;

        if (_tickCounter >= 20)
        {
            _tickCounter = 0;
            foreach (Collider targetPlayer in allTargets)
            {
                SpawnProjector(targetPlayer);
            }
        }
        _isAttackReady = _controller.SetAnimationSpeed(_elapsedTime, _animLength, _controller.BossSkill1State,out float animSpeed, START_SKILL1_ANIM_SPEED);
        if (_isAttackReady)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }



    private void SpawnProjector(Collider targetPlayer)
    {
        Indicator_Controller projector = Managers.ResourceManager.Instantiate("Prefabs/Enemy/Boss/Indicator/Boss_Skill1_Indicator").GetComponent<Indicator_Controller>();
        Managers.RelayManager.SpawnNetworkOBJ(projector.gameObject);
        //projector.transform.SetParent(Managers.VFX_Manager.VFX_Root_NGO, false);
        projector.transform.position = targetPlayer.transform.position;
        //projector.SetValue(2, 360);
        //projector.FillProgress = 0;
        StartCoroutine(startProjector(projector, targetPlayer));
    }




    private IEnumerator startProjector(Indicator_Controller projector, Collider targetPlayer)
    {
        SpawnStone(targetPlayer);
        float elaspedTime = 0f;
        while (elaspedTime < _attackDelayTime)
        {
            elaspedTime += Time.deltaTime;
            float fillRatio = Mathf.Clamp01(elaspedTime / _attackDelayTime);

            // 인디케이터 채우기
            //projector.FillProgress = fillRatio;
            //projector.UpdateProjectors();

            yield return null;
        }
        TargetInSight.AttackTargetInCircle(projector.GetComponent<ProjectorAttack>(), projector.Radius, Damage.Value);
        Managers.ResourceManager.DestroyObject(projector.gameObject);
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