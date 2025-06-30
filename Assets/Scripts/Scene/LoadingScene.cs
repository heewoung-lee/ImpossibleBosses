using System.Collections;
using System.Threading.Tasks;
using GameManagers;
using GameManagers.Interface.UIManager;
using Scene.GamePlayScene;
using UI.Scene.SceneUI;
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
        [Inject] SceneManagerEx _sceneManagerEx;
        
        
        UILoading _uiLoading;
        public override Define.Scene CurrentScene => Define.Scene.LoadingScene;
        public override ISceneSpawnBehaviour SceneSpawnBehaviour { get; }
        public bool IsErrorOccurred { get; set; } = false;
        private bool[] _isCheckTaskChecker;
        protected override Task StartInit()
        {
            base.StartInit();
            _uiLoading = _uiManager.GetSceneUIFromResource<UILoading>();
            _isCheckTaskChecker = _sceneManagerEx.LoadingSceneTaskChecker;
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
            AsyncOperation operation = SceneManager.LoadSceneAsync(_sceneManagerEx.NextScene.ToString());
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
