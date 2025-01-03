using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillComponent : UI_Base
{
    
    enum SkillIamge
    {
        SkillIconImage,
        CoolTimeIMG
    }
    private BaseSkill _skill;
    public BaseSkill Skill { get => _skill; }


    private Image _iconimage;
    private Image _coolTimeIMG;
    private float _coolTime;
    private bool _isSkillReady;

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
        _coolTimeIMG = Get<Image>((int)SkillIamge.CoolTimeIMG);
        BindEvent(_iconimage.gameObject, ClicktoSkill);
        _isSkillReady = true;
    }


    public void ClicktoSkill(PointerEventData eventdata)
    {
        SkillStart();
    }
    
    public void SkillStart()
    {
        if (_isSkillReady)
        {
            StartCoroutine(TriggerCooldown());
            _skill.InvokeSkill();
            //이부분에 버프류 스킬이면 Duration이 돌아가게 만들어야함
        }
    }

    private IEnumerator TriggerCooldown()
    {
        _coolTimeIMG.fillAmount = 1;
        _isSkillReady = false;
        while (_coolTimeIMG.fillAmount > 0)
        {
            _coolTimeIMG.fillAmount -= Time.deltaTime / _coolTime;
            yield return null;  
        }
        _coolTimeIMG.fillAmount = 0;
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

    }

}