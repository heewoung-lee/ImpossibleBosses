using GameManagers;
using UI.Scene.SceneUI;
using UnityEngine;
using Zenject;

namespace Module.EnemyModule
{
    public class ModuleBossHpUI : MonoBehaviour
    {
        [Inject] private UIManager _uiManager;

        private void Start()
        {
            UIBossHp bossHpUI = _uiManager.GetSceneUIFromResource<UIBossHp>();
        }

    }
}
