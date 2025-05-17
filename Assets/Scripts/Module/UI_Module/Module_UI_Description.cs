using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_UI_Description : MonoBehaviour
{
    UI_Description _description;
    void Awake()
    {
        _description = Managers.UI_Manager.GetSceneUIFromResource<UI_Description>();
        
    }
    private void Start()
    {
        _description.GetComponent<Canvas>().sortingOrder =
            Managers.UI_Manager.GetImportant_Popup_UI<UI_Player_Inventory>().GetComponent<Canvas>().sortingOrder + 1;
    }

}
