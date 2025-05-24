using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlaySceneLoadingProgress : UI_Base
{
    private UI_Loading _ui_loding;
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
        _ui_loding = GetComponent<UI_Loading>();
    }

    protected override void StartInit()
    {
        _totalPlayerCount = Managers.RelayManager.CurrentUserCount;


        if(Managers.RelayManager.NGO_RPC_Caller == null)
        {
            Managers.RelayManager.Spawn_RpcCaller_Event += LoadPlayerInit;
        }
        else
        {
            LoadPlayerInit();
        }
        void LoadPlayerInit()
        {
            LoadedPlayerCount = Managers.RelayManager.NGO_RPC_Caller.LoadedPlayerCount;
            SetisAllPlayerLoaded(Managers.RelayManager.NGO_RPC_Caller.IsAllPlayerLoaded);
        }

    }

    private IEnumerator LoadingSceneProcess(int playerCount)
    {
        float pretimer = 0f;
        float aftertimer = 0f;
        float processLength = 0.9f / _totalPlayerCount;
        while (_ui_loding.LoaingSliderValue <= 1f)
        {
            yield return null;
            if (_ui_loding.LoaingSliderValue < 0.9f)
            {
                int sucessCount = LoadedPlayerCount;
                _ui_loding.LoaingSliderValue = sucessCount * processLength;
                pretimer += Time.deltaTime / 5f;
                _ui_loding.LoaingSliderValue = Mathf.Lerp(_ui_loding.LoaingSliderValue - processLength, _ui_loding.LoaingSliderValue + processLength, pretimer);
            }
            else if(_ui_loding.LoaingSliderValue >= 0.9f && _isAllPlayerLoaded == true)
            {
                aftertimer += Time.deltaTime / 2f;
                _ui_loding.LoaingSliderValue = Mathf.Lerp(0.9f, 1, aftertimer);
                if (_ui_loding.LoaingSliderValue >= 1.0f)
                {
                    StartCoroutine(FadeOutLoadedScene());
                    //알파값 내려가고, 다 내려가면 열려야함.
                    yield break;
                }
            }
        }
    }

    private IEnumerator FadeOutLoadedScene()
    {
        float loadSceneImageAlpha = 1f;
        Image[] loadSceneImages = _ui_loding.GetComponentsInChildren<Image>();

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

        _ui_loding.gameObject.SetActive(false);
        _onLoadingComplete?.Invoke();
        foreach (Image loadsceneImage in loadSceneImages)
        {
            Color currentColor = loadsceneImage.color;
            loadsceneImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
        }

        Managers.RelayManager.NGO_RPC_Caller.LoadedPlayerCount = 0;
        _loadedPlayerCount = 0;
    }
}
