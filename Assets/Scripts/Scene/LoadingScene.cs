using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using GameManagers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : BaseScene
{
    UI_Loading _ui_loding;

    public override Define.Scene CurrentScene => Define.Scene.LoadingScene;
    public bool IsErrorOccurred { get; set; } = false;

    private bool[] _isCheckTaskChecker;


    public bool[] LobbyTask;

    protected override void StartInit()
    {
        base.StartInit();
        _isCheckTaskChecker = Managers.SceneManagerEx.LoadingSceneTaskChecker;
        LobbyTask = Managers.LobbyManager.TaskChecker;
        StartCoroutine(LoadingSceneProcess());
    }

    public override void Clear()
    {
    }

    protected override void AwakeInit()
    {
        _ui_loding = Managers.UIManager.GetSceneUIFromResource<UI_Loading>();
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

            if (_ui_loding.LoaingSliderValue < 0.9f)
            {
                int sucessCount = 0;
                foreach (bool OperationSucess in _isCheckTaskChecker)
                {
                    if (OperationSucess is true)
                    {
                        sucessCount++;
                    }
                }
                _ui_loding.LoaingSliderValue = sucessCount * processLength;
                pretimer += Time.deltaTime / 5f;
                _ui_loding.LoaingSliderValue = Mathf.Lerp(_ui_loding.LoaingSliderValue - processLength, _ui_loding.LoaingSliderValue + processLength, pretimer);
            }
            else
            {
                aftertimer += Time.deltaTime/5f;
                _ui_loding.LoaingSliderValue = Mathf.Lerp(0.9f,1, aftertimer);
                if (_ui_loding.LoaingSliderValue>=1.0f)
                {
                    operation.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
