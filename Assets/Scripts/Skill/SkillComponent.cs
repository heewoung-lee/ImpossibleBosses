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
            //�̺κп� ������ ��ų�̸� Duration�� ���ư��� ��������
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
        go.GetComponent<RectTransform>().anchorMin = Vector2.zero; // ���� �ϴ� (0, 0)
        go.GetComponent<RectTransform>().anchorMax = Vector2.one;  // ���� ��� (1, 1)
        go.GetComponent<RectTransform>().offsetMin = Vector2.zero; // ������ ����
        go.GetComponent<RectTransform>().offsetMax = Vector2.zero; // ������ ����
    }

    protected override void StartInit()
    {

    }

}