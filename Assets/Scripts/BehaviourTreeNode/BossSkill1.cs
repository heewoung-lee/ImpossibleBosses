using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using UnityEngine;

public class BossSkill1 : Action
{
    public SharedInt Damage;

    private BossGolemController _controller;
    private BossStats _stats;
   
    private float _elapsedTime = 0f;
    private int _tickCounter = 0;
    private float _animLength = 0f;

    private bool _isAttackReady = false;
    private Collider[] allTargets;


    public override void OnStart()
    {
        base.OnStart();
        _controller = Owner.GetComponent<BossGolemController>();
        _stats = _controller.GetComponent<BossStats>();
        _animLength = Utill.GetAnimationLength("Anim_Hit", _controller.Anim);
        allTargets = Physics.OverlapSphere(Owner.transform.position,float.MaxValue,_stats.TarGetLayer);
        _controller.CurrentStateType = _controller.BossSkill1State;
    }

    public override TaskStatus OnUpdate()
    {
        _elapsedTime += Time.deltaTime * _controller.Anim.speed;
        _tickCounter++;

        if (_tickCounter >= 100)
        {
            _tickCounter = 0;
            foreach(Collider targetPlayer in allTargets)
            {
                Indicator_Controller projector = Managers.ResourceManager.Instantiate("Prefabs/Enemy/Boss/Indicator/Boss_Skill1_Indicator").GetComponent<Indicator_Controller>();
                projector.transform.SetParent(Managers.VFX_Manager.VFX_Root,false);
                projector.transform.position = targetPlayer.transform.position;
                projector.SetValue(2, 360);
                projector.FillProgress = 0;
                StartCoroutine(startProjector(projector));
            }
        }
        _isAttackReady = _controller.SetAnimationSpeed(_elapsedTime, _animLength, _controller.BossSkill1State, 0.8f);
        if (_isAttackReady)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }

    private IEnumerator startProjector(Indicator_Controller projector)
    {
        float elaspedTime = 0f;
        GameObject stone = Managers.ResourceManager.Instantiate("Prefabs/Enemy/Boss/AttackPattren/BossSkill1");
        stone.transform.SetParent(Managers.VFX_Manager.VFX_Root, false);
        stone.transform.position = projector.transform.position + Vector3.up*5f;
        while (projector.FillProgress < 1)
        {
            elaspedTime += Time.deltaTime;
            projector.FillProgress = elaspedTime;
            projector.UpdateProjectors();
            yield return null;
        }
        TargetInSight.AttackTargetInCircle(projector.GetComponent<ProjectorAttack>(), projector.Radius, Damage.Value);
        Managers.ResourceManager.DestroyObject(projector.gameObject);
        Managers.ResourceManager.DestroyObject(stone,2f);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        _elapsedTime = 0;
        _controller.Anim.speed = 1f;
    }
}