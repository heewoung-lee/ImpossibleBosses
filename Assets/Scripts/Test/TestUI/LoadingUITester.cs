using System;
using System.Collections;
using UI.Scene.SceneUI;
using UnityEngine;

namespace Test.TestUI
{
    public class LoadingUITester : MonoBehaviour
    {

        public bool IsCheckTest;
        private UILoading _uiLoading;
        Action _isdoneLoadLoadingUI;

        WaitUntil _waitUntil;

        void Start()
        {
            if (IsCheckTest is true)
            {
                _waitUntil = new WaitUntil(IscheckUILoadingActive);
                StartCoroutine(TestLoadingUI());


            }

            bool IscheckUILoadingActive()
            {
                if (TryGetComponent(out UILoading uiloing) == true)
                {
                    _uiLoading  = uiloing;
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
            yield return _waitUntil;
            _uiLoading.gameObject.SetActive(false);
        }

    }
}
