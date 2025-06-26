using System;
using GameManagers.Interface.InputManager;
using Skill;
using UnityEngine;
using UnityEngine.InputSystem;
using Util;
using Zenject;

namespace UI.Scene.SceneUI
{
    public class UISkillBar : UIScene
    {
        private Transform[] _skillContextFrames;
        private SkillSlot[] _skillSlot;
        private InputAction _getQKey;
        private InputAction _getWKey;
        private InputAction _getEKey;
        private InputAction _getRKey;

        [Inject] private IInputAsset _inputManager;

        enum SkillICons
        {
            SkillContextFrame1,
            SkillContextFrame2,
            SkillContextFrame3,
            SkillContextFrame4
        }

        protected override void AwakeInit()
        {
            base.AwakeInit();
            _skillContextFrames = new Transform[Enum.GetValues(typeof(SkillICons)).Length];
            _skillSlot = new SkillSlot[Enum.GetValues(typeof(SkillICons)).Length];
            Bind<Transform>(typeof(SkillICons));
            SkillICons[] skillIcons = (SkillICons[])System.Enum.GetValues(typeof(SkillICons));
            for (int i = 0; i < _skillContextFrames.Length; i++)
            {
                _skillContextFrames[i] = Get<Transform>((int)skillIcons[i]);
                _skillSlot[i] = _skillContextFrames[i].GetComponent<SkillSlot>();
            }
            BindKeyBoard();
        }

        public Transform SetLocationSkillSlot(SkillComponent skillcomponent)
        {
            foreach(Transform skillFrameTr in _skillContextFrames)
            {
                SkillSlot skillslot = skillFrameTr.GetComponent<SkillSlot>();

                if (skillslot.SkillComponent == null)
                {
                    skillslot.SkillComponent = skillcomponent;
                    return skillFrameTr;
                }
            }
            Debug.LogError("Skill slots are full.");
            return null;
        }

        public void BindKeyBoard()
        {
            _getQKey = _inputManager.GetInputAction(Define.ControllerType.UI, "SkillBar_GetKeyQ");
            _getWKey = _inputManager.GetInputAction(Define.ControllerType.UI, "SkillBar_GetKeyW");
            _getEKey = _inputManager.GetInputAction(Define.ControllerType.UI, "SkillBar_GetKeyE");
            _getRKey = _inputManager.GetInputAction(Define.ControllerType.UI, "SkillBar_GetKeyR");

            _getQKey.Enable();
            _getWKey.Enable();
            _getEKey.Enable();
            _getRKey.Enable();

            _getQKey.started += GetKey;
            _getWKey.started += GetKey;
            _getEKey.started += GetKey;
            _getRKey.started += GetKey;
        }
        private void OnDisable()
        {
            _getQKey.started -= GetKey;
            _getWKey.started -= GetKey;
            _getEKey.started -= GetKey;
            _getRKey.started -= GetKey;
        }
        public void GetKey(InputAction.CallbackContext context)
        {
            switch (context.control.name)
            {
                case "q":
                    if (_skillContextFrames[(int)SkillICons.SkillContextFrame1].GetComponent<SkillSlot>().SkillComponent == null)
                        return;
                    _skillSlot[(int)SkillICons.SkillContextFrame1].SkillComponent.SkillStart();
                    break;
                case "w":
                    if (_skillContextFrames[(int)SkillICons.SkillContextFrame2].GetComponent<SkillSlot>().SkillComponent == null)
                        return;
                    _skillSlot[(int)SkillICons.SkillContextFrame2].SkillComponent.SkillStart();
                    break;
                case "e":
                    if (_skillContextFrames[(int)SkillICons.SkillContextFrame3].GetComponent<SkillSlot>().SkillComponent == null)
                        return;
                    _skillSlot[(int)SkillICons.SkillContextFrame3].SkillComponent.SkillStart();
                    break;
                case "r":
                    if (_skillContextFrames[(int)SkillICons.SkillContextFrame4].GetComponent<SkillSlot>().SkillComponent == null)
                        return;
                    _skillSlot[(int)SkillICons.SkillContextFrame4].SkillComponent.SkillStart();
                    break;
            }
        }


        protected override void StartInit()
        {

        }
    }

}