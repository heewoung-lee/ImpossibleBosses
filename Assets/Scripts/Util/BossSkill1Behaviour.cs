using UnityEngine;

public class BossSkill1Behaviour : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.speed = 0f;
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.speed = 1f;
        Debug.Log($"Enter 상태: {stateInfo.shortNameHash}");
        float normalized = stateInfo.normalizedTime;
        Debug.Log($"애니메이션 시작지점 {normalized}");
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        Debug.Log($"Exit 상태: {stateInfo.shortNameHash}");
        float normalized = stateInfo.normalizedTime;
        Debug.Log($"애니메이션 끝지점 {normalized}");
        animator.speed = 0;
    }
}