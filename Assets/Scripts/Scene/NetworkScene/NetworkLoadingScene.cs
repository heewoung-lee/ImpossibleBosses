using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkLoadingScene : BaseNetworkScene
{
    UI_Loading _ui_loding;

    private static Define.Scene _nextScene;
    public override Define.Scene CurrentScene => Define.Scene.LoadingScene;
    public bool IsErrorOccurred { get; set; } = false;

    private static bool[] _isCheckTaskChecker;


    [Rpc(SendTo.ClientsAndHost)]
    public void CallLoadNextSceneRpc(string nextSceneName)
    {
        _nextScene = (Define.Scene)Enum.Parse(typeof(Define.Scene), nextSceneName);
    }
    protected override void StartInit()
    {
        base.StartInit();
        StartCoroutine(LoadingSceneProcess());
    }

    public override void Clear()
    {
    }

    protected override void AwakeInit()
    {
        _ui_loding = Managers.UI_Manager.GetSceneUIFromResource<UI_Loading>();
    }

    public static void SetCheckTaskChecker(bool[] CheckTaskChecker)
    {
        _isCheckTaskChecker = CheckTaskChecker;
    }

    private IEnumerator LoadingSceneProcess()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(_nextScene.ToString());
        operation.allowSceneActivation = false;
        while (_isCheckTaskChecker == null)
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
                aftertimer += Time.deltaTime / 5f;
                _ui_loding.LoaingSliderValue = Mathf.Lerp(0.9f, 1, aftertimer);
                if (_ui_loding.LoaingSliderValue >= 1.0f)
                {
                    operation.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
