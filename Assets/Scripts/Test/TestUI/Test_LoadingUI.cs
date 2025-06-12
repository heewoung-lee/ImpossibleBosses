using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Test_LoadingUI : MonoBehaviour
{

    public bool IsCheckTest;

    private UI_Loading _ui_Loading;
    Action isdoneLoadLoadingUI;

    WaitUntil waitUntil;

    void Start()
    {
        if (IsCheckTest is true)
        {
            waitUntil = new WaitUntil(ischeckUI_LoadingActive);
            StartCoroutine(TestLoadingUI());


        }

        bool ischeckUI_LoadingActive()
        {
            if (TryGetComponent(out UI_Loading uiloing) == true)
            {
                _ui_Loading  = uiloing;
                return true;
            }
            else
            {
                return false;
            }
        }

    }

    IEnumerator TestLoadingUI()
    {
        yield return waitUntil;
        _ui_Loading.gameObject.SetActive(false);
    }

}
