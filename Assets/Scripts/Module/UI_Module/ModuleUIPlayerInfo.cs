using GameManagers;
using UnityEngine;
using Zenject;

namespace Module.UI_Module
{
    public class ModuleUIPlayerInfo : MonoBehaviour
    {
        [Inject] private UIManager _uimanager;
        void Start()
        {
            UI_Player_Info playerInfoUI = _uimanager.GetSceneUIFromResource<UI_Player_Info>();
        }
    }
}
