using GameManagers;
using UnityEngine;

namespace Module.EnemyModule
{
    public class ModuleBossHpUI : MonoBehaviour
    {
        private void Start()
        {
            UI_Boss_HP bossHpUI = Managers.UIManager.GetSceneUIFromResource<UI_Boss_HP>();
        }

    }
}
