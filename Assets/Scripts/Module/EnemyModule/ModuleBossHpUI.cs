using GameManagers;
using UnityEngine;
using Zenject;

namespace Module.EnemyModule
{
    public class ModuleBossHpUI : MonoBehaviour
    {
        [Inject] private UIManager _uiManager;

        private void Start()
        {
            UI_Boss_HP bossHpUI = _uiManager.GetSceneUIFromResource<UI_Boss_HP>();
        }

    }
}
