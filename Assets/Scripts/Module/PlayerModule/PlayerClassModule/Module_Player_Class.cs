using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Module_Player_Class : MonoBehaviour
{
    public abstract Define.PlayerClass PlayerClass { get; }


    private Dictionary<string, BaseSkill> _playerSkill;

    public virtual void InitAwake()
    {

    }

    public virtual void InitStart()
    {
        _playerSkill = Managers.SkillManager.AllSKillDict
            .Where(skill => skill.Value.PlayerClass == PlayerClass)
            .ToDictionary(skill => skill.Key, skill => skill.Value);//각 클래스에 맞는 스킬들을 추린다

        if(Managers.SkillManager.UI_SkillBar == null)
        {
            Managers.SkillManager.Done_UI_SKilBar_Init_Event += InitlizeSkilintoSlot;
        }
        else
        {
            InitlizeSkilintoSlot();
        }
    }



    public void InitlizeSkilintoSlot()
    {
        foreach (BaseSkill skill in _playerSkill.Values)
        {
            GameObject skillPrefab = Managers.ResourceManager.InstantiatePrefab("UI/Skill/UI_SkillComponent");
            SkillComponent skillcomponent = skillPrefab.GetOrAddComponent<SkillComponent>();
            skillcomponent.SetSkillComponent(skill);
            Transform skillLocation = Managers.SkillManager.UI_SkillBar.SetLocationSkillSlot(skillcomponent);
            skillcomponent.AttachItemToSlot(skillcomponent.gameObject, skillLocation);
        }
    }
    private void Awake()
    {
        _playerSkill = new Dictionary<string, BaseSkill>();
        InitAwake();
    }

    private void Start()
    {
        InitStart();
    }
}
