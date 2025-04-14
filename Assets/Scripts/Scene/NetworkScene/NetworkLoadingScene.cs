using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkLoadingScene : BaseScene
{
    UI_Loading _ui_loding;

    private Define.Scene _nextScene;
    public override Define.Scene CurrentScene => Define.Scene.NetworkLoadingScene;
    public bool IsErrorOccurred { get; set; } = false;

    private bool[] _isCheckTaskChecker;
    private int taskIndex = 0;

    private GameObject _networkLoadingRpc;
    protected override async void StartInit()
    {
        base.StartInit();
        HostSpawn();
        await LeaveLobbyAndVivox();
        Managers.SceneManagerEx.SetCurrentScene(CurrentScene);
        Managers.SceneManagerEx.SetNextScene(Define.Scene.GamePlayScene);
        async Task LeaveLobbyAndVivox()
        {
            await Managers.LobbyManager.LeaveCurrentLobby();
            await Managers.VivoxManager.LogoutOfVivoxAsync();
        }
        void HostSpawn()
        {
            _networkLoadingRpc = Managers.RelayManager.SpawnNetworkOBJ("Prefabs/NGO/Scene_NGO/NGO_LoadingScene_RPC_Caller");
        }
    }


    public override void Clear()
    {
    }

    protected override void AwakeInit()
    {
        _ui_loding = Managers.UI_Manager.GetSceneUIFromResource<UI_Loading>();
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
                        //DoneMyTaskCalltoHostRpc();
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
