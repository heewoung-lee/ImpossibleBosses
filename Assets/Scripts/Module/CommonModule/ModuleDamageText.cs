using GameManagers;
using Stats.BaseStats;
using UI.WorldSpace;
using UnityEngine;

namespace Module.CommonModule
{
    public class ModuleDamageText : MonoBehaviour
    {
        private void Start()
        {
            BaseStats stat = GetComponent<BaseStats>();
            stat.EventAttacked += ShowDamageText_UI;
        }
        public void ShowDamageText_UI(int damage, int currentHp)
        {
            UIDamageText uIDamageText = Managers.UIManager.MakeUIWorldSpaceUI<UIDamageText>();
            uIDamageText.SetTextAndPosition(transform, damage);
            uIDamageText.transform.SetParent(transform);
        }
    }
}
