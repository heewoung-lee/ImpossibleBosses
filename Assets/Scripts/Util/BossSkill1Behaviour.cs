using UnityEngine;

public class BossSkill1Behaviour : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.speed = 0f;
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.speed = 1f;
        Debug.Log($"Enter ����: {stateInfo.shortNameHash}");
        float normalized = stateInfo.normalizedTime;
        Debug.Log($"�ִϸ��̼� �������� {normalized}");
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        Debug.Log($"Exit ����: {stateInfo.shortNameHash}");
        float normalized = stateInfo.normalizedTime;
        Debug.Log($"�ִϸ��̼� ������ {normalized}");
        animator.speed = 0;
    }
}