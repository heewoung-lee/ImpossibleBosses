using System;
using System.Collections;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkLoadingScene : BaseNetworkScene
{
    UI_Loading _ui_loding;

    private Define.Scene _nextScene;
    public override Define.Scene CurrentScene => Define.Scene.NetworkLoadingScene;
    public bool IsErrorOccurred { get; set; } = false;

    private bool[] _isCheckTaskChecker;
    private int taskIndex = 0;
    


    [Rpc(SendTo.ClientsAndHost)]
    public void CallLoadNextSceneRpc(string nextSceneName)
    {
        _nextScene = (Define.Scene)Enum.Parse(typeof(Define.Scene), nextSceneName);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        CallLoadNextSceneRpc(Managers.SceneManagerEx.NextSceneName.ToString());
        _isCheckTaskChecker = new bool[Managers.RelayManager.NetworkManagerEx.ConnectedClientsList.Count];
        StartCoroutine(LoadingSceneProcess());
    }

    public override void Clear()
    {
    }

    protected override void AwakeInit()
    {
        _ui_loding = Managers.UI_Manager.GetSceneUIFromResource<UI_Loading>();
    }

    [Rpc(SendTo.Server)]
    public void DoneMyTaskCalltoHostRpc()
    {
        _isCheckTaskChecker[taskIndex] = true;
        UserTaskDoneClientRpc(taskIndex);
        taskIndex++;

    }
    [Rpc(SendTo.ClientsAndHost)]
    public void UserTaskDoneClientRpc(int taskdoneIndex)
    {
        _isCheckTaskChecker[taskIndex] = true;
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
                        DoneMyTaskCalltoHostRpc();
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
