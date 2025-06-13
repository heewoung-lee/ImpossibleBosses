using Skill;
using UnityEngine;

public class SkillSlot : MonoBehaviour
{
    [SerializeField]private SkillComponent _skillComponent;
    public SkillComponent SkillComponent { get => _skillComponent; set => _skillComponent = value; }
}
