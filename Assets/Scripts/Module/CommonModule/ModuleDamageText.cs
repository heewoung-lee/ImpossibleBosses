using GameManagers;
using Stats.BaseStats;
using UI.WorldSpace;
using UnityEngine;
using Zenject;

namespace Module.CommonModule
{
    public class ModuleDamageText : MonoBehaviour
    {
        [Inject] private UIManager _uiManager;
        private void Start()
        {
            BaseStats stat = GetComponent<BaseStats>();
            stat.EventAttacked += ShowDamageText_UI;
        }
        public void ShowDamageText_UI(int damage, int currentHp)
        {
            UIDamageText uIDamageText = _uiManager.MakeUIWorldSpaceUI<UIDamageText>();
            uIDamageText.SetTextAndPosition(transform, damage);
            uIDamageText.transform.SetParent(transform);
        }
    }
}
