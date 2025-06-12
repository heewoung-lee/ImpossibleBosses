using GameManagers;
using UnityEngine;

namespace Module.CommonModule
{
    public class ModuleDamageText : MonoBehaviour
    {
        private void Start()
        {
            BaseStats stat = GetComponent<BaseStats>();
            stat.Event_Attacked += ShowDamageText_UI;
        }
        public void ShowDamageText_UI(int damage, int currentHp)
        {
            UI_DamageText uIDamageText = Managers.UIManager.MakeUIWorldSpaceUI<UI_DamageText>();
            uIDamageText.SetTextAndPosition(transform, damage);
            uIDamageText.transform.SetParent(transform);
        }
    }
}
