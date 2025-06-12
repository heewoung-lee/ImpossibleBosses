using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace Module.PlayerModule
{
    public class ModulePlayerAnimInfo : MonoBehaviour
    {
        Dictionary<int, AnimationClip> _playerAnimaInfoDict;
        //애니메이터 를 가져온다음
        //애니메이터를 반복문을 돌려 animClip을 가져옴
        //클립마다 해쉬 값과 같이 딕셔너리에 저장
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
                {//각 애니메이터의 레이어에 접근
                    foreach (ChildAnimatorState state in layer.stateMachine.states)
                    {//레이어에 있는 state를 조사
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
}