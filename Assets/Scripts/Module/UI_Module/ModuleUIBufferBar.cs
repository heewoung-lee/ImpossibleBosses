using GameManagers;
using UnityEngine;
using Zenject;

namespace Module.UI_Module
{
    public class ModuleUIBufferBar : MonoBehaviour
    {
        [Inject]private UIManager _uiManager; 
        UI_BufferBar _uiBufferbar;

        void Start()
        {
            _uiBufferbar = _uiManager.GetSceneUIFromResource<UI_BufferBar>();
        }
    }
}
