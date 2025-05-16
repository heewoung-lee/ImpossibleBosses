using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Module_UI_BufferBar : MonoBehaviour
{
    UI_BufferBar _ui_bufferbar;

    void Start()
    {
        _ui_bufferbar = Managers.UI_Manager.GetSceneUIFromResource<UI_BufferBar>(isCheckDontDestroy:true);
    }
}
