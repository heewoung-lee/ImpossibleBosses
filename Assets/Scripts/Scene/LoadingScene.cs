using System.Collections;
using GameManagers;
using GameManagers.Interface.UIManager;
using Scene.GamePlayScene;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;
using Zenject;

namespace Scene
{
    public class LoadingScene : BaseScene
    {
        [Inject]private IUISceneManager _uiManager; 
        
        
        
        UI_Loading _uiLoading;
        public override Define.Scene CurrentScene => Define.Scene.LoadingScene;
        public override ISceneSpawnBehaviour SceneSpawnBehaviour { get; }
        public bool IsErrorOccurred { get; set; } = false;
        private bool[] _isCheckTaskChecker;
        protected override void StartInit()
        {
            base.StartInit();
            _uiLoading = _uiManager.GetSceneUIFromResource<UI_Loading>();
            _isCheckTaskChecker = Managers.SceneManagerEx.LoadingSceneTaskChecker;
            StartCoroutine(LoadingSceneProcess());
        }

        public override void Clear()
        {
        }

        protected override void AwakeInit()
        {
          
        }


        private IEnumerator LoadingSceneProcess()
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(Managers.SceneManagerEx.NextScene.ToString());
            operation.allowSceneActivation = false;
            while(_isCheckTaskChecker == null)
            {
                yield return null;
            }
            float pretimer = 0f;
            float aftertimer = 0f;
            float processLength = 0.9f / _isCheckTaskChecker.Length;
        
            while (operation.isDone == false)
            {
                yield return null;

                if (IsErrorOccurred == true)
                    yield break;

                if (_uiLoading.LoaingSliderValue < 0.9f)
                {
                    int sucessCount = 0;
                    foreach (bool operationSucess in _isCheckTaskChecker)
                    {
                        if (operationSucess is true)
                        {
                            sucessCount++;
                        }
                    }
                    _uiLoading.LoaingSliderValue = sucessCount * processLength;
                    pretimer += Time.deltaTime / 5f;
                    _uiLoading.LoaingSliderValue = Mathf.Lerp(_uiLoading.LoaingSliderValue - processLength, _uiLoading.LoaingSliderValue + processLength, pretimer);
                }
                else
                {
                    aftertimer += Time.deltaTime/5f;
                    _uiLoading.LoaingSliderValue = Mathf.Lerp(0.9f,1, aftertimer);
                    if (_uiLoading.LoaingSliderValue>=1.0f)
                    {
                        operation.allowSceneActivation = true;
                        yield break;
                    }
                }
            }
        }
    }
}
