using GameManagers;
using UnityEngine;

namespace UI.Scene
{
    public class UIScene : UIBase
    {
        protected override void AwakeInit()
        {
            Managers.UIManager.SetCanvas(gameObject.GetComponent<Canvas>(), true);
        }
        protected override void StartInit()
        {

        }
    }
}
