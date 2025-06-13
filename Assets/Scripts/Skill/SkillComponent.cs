using System.Collections;
using GameManagers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Skill
{
    public class SkillComponent : UI_Base
    {
    
        enum SkillIamge
        {
            SkillIconImage,
            CoolTimeImg
        }
        private BaseSkill _skill;
        public BaseSkill Skill { get => _skill; }


        private Image _iconimage;
        private Image _coolTimeImg;
        private float _coolTime;
        private bool _isSkillReady;
        private UI_Description _decriptionObject;
        private RectTransform _skillComponentRectTr;
        public void SetSkillComponent(BaseSkill skill)
        {
            _skill = skill;
            _iconimage.sprite = _skill.SkillconImage;
            _coolTime = _skill.CoolTime;
        }

        protected override void AwakeInit()
        {
            Bind<Image>(typeof(SkillIamge));
            _iconimage = Get<Image>((int)SkillIamge.SkillIconImage);
            _coolTimeImg = Get<Image>((int)SkillIamge.CoolTimeImg);
            _isSkillReady = true;
            _skillComponentRectTr = transform as RectTransform;
            BindEvent(_iconimage.gameObject, ClicktoSkill);
            BindEvent(gameObject, ShowDescription, Define.UI_Event.PointerEnter);
            BindEvent(gameObject, CloseDescription, Define.UI_Event.PointerExit);
        }
        private void ShowDescription(PointerEventData data)
        {
            _decriptionObject.UI_DescriptionEnable();

            _decriptionObject.DescriptionWindow.transform.position
                = _decriptionObject.SetDecriptionPos(transform, _skillComponentRectTr.rect.width, _skillComponentRectTr.rect.height);

            _decriptionObject.SetValue(_skill.SkillconImage,_skill.SkillName);
            _decriptionObject.SetItemEffectText(_skill.EffectDescriptionText);
            _decriptionObject.SetDescription(_skill.ETCDescriptionText);
        }
        private void CloseDescription(PointerEventData data)
        {
            _decriptionObject.UI_DescriptionDisable();
            _decriptionObject.SetdecriptionOriginPos();
        }

        public void ClicktoSkill(PointerEventData eventdata)
        {
            SkillStart();
        }
    
        public void SkillStart()
        {
            if (_isSkillReady&& _skill.IsStateUpdatedAfterSkill())
            {
                StartCoroutine(TriggerCooldown());
            }
        }

        private IEnumerator TriggerCooldown()
        {
            _coolTimeImg.fillAmount = 1;
            _isSkillReady = false;
            while (_coolTimeImg.fillAmount > 0)
            {
                _coolTimeImg.fillAmount -= Time.deltaTime / _coolTime;
                yield return null;  
            }
            _coolTimeImg.fillAmount = 0;
            _isSkillReady = true;
        }

        public void AttachItemToSlot(GameObject go, Transform slot)
        {
            go.transform.SetParent(slot);
            go.GetComponent<RectTransform>().anchorMin = Vector2.zero; // 좌측 하단 (0, 0)
            go.GetComponent<RectTransform>().anchorMax = Vector2.one;  // 우측 상단 (1, 1)
            go.GetComponent<RectTransform>().offsetMin = Vector2.zero; // 오프셋 제거
            go.GetComponent<RectTransform>().offsetMax = Vector2.zero; // 오프셋 제거
        }
        protected override void StartInit()
        {
            _decriptionObject = Managers.UIManager.Get_Scene_UI<UI_Description>();
        
        }

    }
}