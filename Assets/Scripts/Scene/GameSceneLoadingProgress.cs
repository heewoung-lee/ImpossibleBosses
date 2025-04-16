using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneLoadingProgress : UI_Base
{
    UI_Loading _ui_loding;

    private int _loadedPlayerCount = 0;

    public int LoadedPlayerCount => _loadedPlayerCount;


    public void SetLoadedPlayerCount(int doneCount)
    {
        _loadedPlayerCount = doneCount;
    }

    protected override void AwakeInit()
    {
        _ui_loding = GetComponent<UI_Loading>();
    }

    protected override void StartInit()
    {
    }


    //public void StartProGressBar(int playerCount)
    //{
    //    StartCoroutine(LoadingSceneProcess(playerCount));
    //}



    //private IEnumerator LoadingSceneProcess(int playerCount)
    //{
    //    float pretimer = 0f;
    //    float aftertimer = 0f;
    //    float processLength = 0.9f / _isCheckTaskChecker.Length;




    //}


    //private IEnumerator LoadingSceneProcess()
    //{
    //    operation.allowSceneActivation = false;
    //    while(_isCheckTaskChecker == null)
    //    {
    //        yield return null;
    //    }
    //    float pretimer = 0f;
    //    float aftertimer = 0f;
    //    float processLength = 0.9f / _isCheckTaskChecker.Length;

    //    while (operation.isDone == false)
    //    {
    //        yield return null;

    //        if (IsErrorOccurred == true)
    //            yield break;

    //        if (_ui_loding.LoaingSliderValue < 0.9f)
    //        {
    //            int sucessCount = 0;
    //            foreach (bool OperationSucess in _isCheckTaskChecker)
    //            {
    //                if (OperationSucess is true)
    //                {
    //                    sucessCount++;
    //                }
    //            }
    //            _ui_loding.LoaingSliderValue = sucessCount * processLength;
    //            pretimer += Time.deltaTime / 5f;
    //            _ui_loding.LoaingSliderValue = Mathf.Lerp(_ui_loding.LoaingSliderValue - processLength, _ui_loding.LoaingSliderValue + processLength, pretimer);
    //        }
    //        else
    //        {
    //            aftertimer += Time.deltaTime/5f;
    //            _ui_loding.LoaingSliderValue = Mathf.Lerp(0.9f,1, aftertimer);
    //            if (_ui_loding.LoaingSliderValue>=1.0f)
    //            {
    //                operation.allowSceneActivation = true;
    //                yield break;
    //            }
    //        }
    //    }
    //}
}
