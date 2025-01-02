using UnityEngine;
using UnityEngine.UI;

public class SkillComponent : MonoBehaviour
{
    private IBaseSkill _skill;
    public IBaseSkill Skill { get => _skill; }


    private Image _image;

    private float _coolTime;


    private void Start()
    {
        _image.sprite = _skill.SkillconImage;
        _coolTime = _skill.CoolTime;
    }

    public void SetSkillComponent(IBaseSkill skill)
    {
        _skill = skill;
    }
}