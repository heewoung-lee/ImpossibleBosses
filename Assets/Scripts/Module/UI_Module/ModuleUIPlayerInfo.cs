using GameManagers;
using UnityEngine;

namespace Module.UI_Module
{
    public class ModuleUIPlayerInfo : MonoBehaviour
    {
        void Start()
        {
            UI_Player_Info playerInfoUI = Managers.UIManager.GetSceneUIFromResource<UI_Player_Info>();
        }
    }
}
