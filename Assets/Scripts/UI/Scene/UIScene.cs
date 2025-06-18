using GameManagers;
using UnityEngine;
using Zenject;

namespace UI.Scene
{
    public class UIScene : UIBase
    {
        [Inject] private UIManager _uiManager;
        protected override void AwakeInit()
        {
            _uiManager.SetCanvas(gameObject.GetComponent<Canvas>(), true);
        }
        protected override void StartInit()
        {

        }
    }
}
