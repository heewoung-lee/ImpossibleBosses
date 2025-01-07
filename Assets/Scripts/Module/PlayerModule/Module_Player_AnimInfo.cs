using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class Module_Player_AnimInfo : MonoBehaviour
{
    Dictionary<int, AnimationClip> _playerAnimaInfoDict;
    //�ִϸ����� �� �����´���
    //�ִϸ����͸� �ݺ����� ���� animClip�� ������
    //Ŭ������ �ؽ� ���� ���� ��ųʸ��� ����
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    public AnimationClip GetAnimationClip(int animHashCode)
    {
        if (_playerAnimaInfoDict == null)
        {
            _playerAnimaInfoDict = new Dictionary<int, AnimationClip>();
            RuntimeAnimatorController controller = _animator.runtimeAnimatorController;
            AnimatorController animatorController = controller as AnimatorController;

            foreach (AnimatorControllerLayer layer in animatorController.layers)
            {//�� �ִϸ������� ���̾ ����
                foreach (ChildAnimatorState state in layer.stateMachine.states)
                {//���̾ �ִ� state�� ����
                    int stateAnimHash = Animator.StringToHash(state.state.name);
                    if (state.state.motion is AnimationClip clip)
                    {
                        _playerAnimaInfoDict[stateAnimHash] = clip;
                    }
                }
            }
        }
        return _playerAnimaInfoDict[animHashCode];
    }


}