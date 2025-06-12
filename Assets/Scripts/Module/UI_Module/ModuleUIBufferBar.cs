using GameManagers;
using UnityEngine;

namespace Module.UI_Module
{
    public class ModuleUIBufferBar : MonoBehaviour
    {
        UI_BufferBar _uiBufferbar;

        void Start()
        {
            _uiBufferbar = Managers.UIManager.GetSceneUIFromResource<UI_BufferBar>();
        }
    }
}
