using UnityEngine;

namespace Skill
{
    public class SkillSlot : MonoBehaviour
    {
        private SkillComponent _skillComponent;
        public SkillComponent SkillComponent { get => _skillComponent; set => _skillComponent = value; }
    }
}
