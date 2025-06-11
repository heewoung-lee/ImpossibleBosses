using System.Collections;
using System.Collections.Generic;
using GameManagers;
using UnityEngine;

public class Module_UI_Description : MonoBehaviour
{
    UI_Description _description;

    public UI_Description Description
    {
        get
        {
            if(_description == null)
            {
                _description = Managers.UIManager.GetSceneUIFromResource<UI_Description>();
            }

            return _description;
        }
    }
    private void Start()
    {
        Description.GetComponent<Canvas>().sortingOrder = (int)Define.SpecialSortingOrder.Description;
    }
}
