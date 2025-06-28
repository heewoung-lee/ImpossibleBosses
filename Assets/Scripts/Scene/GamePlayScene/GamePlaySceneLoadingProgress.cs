using System;
using System.Collections;
using GameManagers;
using UI;
using UI.Scene.SceneUI;
using UnityEngine;
using UnityEngine.UI;
using Util;
using Zenject;

namespace Scene.GamePlayScene
{
    public class GamePlaySceneLoadingProgress : UIBase
    {
        [Inject] private RelayManager _relayManager;

        
        private UILoading _uiLoading;
        private int _loadedPlayerCount = 0;
        private int _totalPlayerCount = 0;
        private bool _isAllPlayerLoaded = false;
        private Coroutine _loadingProgressCoroutine;
        private Action _onLoadingComplete;


        public event Action OnLoadingComplete
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _onLoadingComplete, value); 
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _onLoadingComplete, value);
            }
        }

        public int LoadedPlayerCount
        {
            get { return _loadedPlayerCount; }
            private set
            {
                if (enabled == false || gameObject.activeInHierarchy == false) 
                    return;

                if (_loadedPlayerCount == value || value <= 0) // 같은 값이거나 0이면 리턴
                    return;

                _loadedPlayerCount = value;

                if (_loadingProgressCoroutine != null)
                    StopCoroutine(_loadingProgressCoroutine);

                _loadingProgressCoroutine =  StartCoroutine(LoadingSceneProcess(_loadedPlayerCount));
            }
        }

        public void SetisAllPlayerLoaded(bool isAllPlayerLoaded)
        {
            _isAllPlayerLoaded =  isAllPlayerLoaded;
        }

        public void SetLoadedPlayerCount(int loadingPlayerCount)
        {
            LoadedPlayerCount = loadingPlayerCount;
        }

        protected override void AwakeInit()
        {
            _uiLoading = GetComponent<UILoading>();
        }

        protected override void StartInit()
        {
            _totalPlayerCount = _relayManager.CurrentUserCount;


            if(_relayManager.NgoRPCCaller == null)
            {
                _relayManager.SpawnRpcCallerEvent += LoadPlayerInit;
            }
            else
            {
                LoadPlayerInit();
            }
            void LoadPlayerInit()
            {
                LoadedPlayerCount = _relayManager.NgoRPCCaller.LoadedPlayerCount;
                SetisAllPlayerLoaded(_relayManager.NgoRPCCaller.IsAllPlayerLoaded);
            }

        }

        private IEnumerator LoadingSceneProcess(int playerCount)
        {
            float pretimer = 0f;
            float aftertimer = 0f;
            float processLength = 0.9f / _totalPlayerCount;
            Image[] loadSceneImages = _uiLoading.GetComponentsInChildren<Image>();
            while (_uiLoading.LoaingSliderValue <= 1f)
            {
                yield return null;
                if (_uiLoading.LoaingSliderValue < 0.9f)
                {
                    int sucessCount = LoadedPlayerCount;
                    _uiLoading.LoaingSliderValue = sucessCount * processLength;
                    pretimer += Time.deltaTime / 5f;
                    _uiLoading.LoaingSliderValue = Mathf.Lerp(_uiLoading.LoaingSliderValue - processLength, _uiLoading.LoaingSliderValue + processLength, pretimer);
                }
                else if(_uiLoading.LoaingSliderValue >= 0.9f && _isAllPlayerLoaded == true)
                {
                    aftertimer += Time.deltaTime / 2f;
                    _uiLoading.LoaingSliderValue = Mathf.Lerp(0.9f, 1, aftertimer);
                    if (_uiLoading.LoaingSliderValue >= 1.0f)
                    {
                        StartCoroutine(FadeOutLoadedScene(loadSceneImages));
                        //알파값 내려가고, 다 내려가면 열려야함.
                        yield break;
                    }
                }
            }
        }

        private IEnumerator FadeOutLoadedScene(Image[] loadSceneImages)
        {
            float loadSceneImageAlpha = 1f;
            while (loadSceneImageAlpha > 0.01f)
            {
                loadSceneImageAlpha -= Time.deltaTime * 2f;

                foreach (Image loadsceneImage in loadSceneImages)
                {
                    Color currentColor = loadsceneImage.color;
                    loadsceneImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, loadSceneImageAlpha);
                }

                yield return null;
            }

            _uiLoading.gameObject.SetActive(false);
            _onLoadingComplete?.Invoke();
            foreach (Image loadsceneImage in loadSceneImages)
            {
                Color currentColor = loadsceneImage.color;
                loadsceneImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
            }

            _relayManager.NgoRPCCaller.LoadedPlayerCount = 0;
            _loadedPlayerCount = 0;
        }
    }
}
