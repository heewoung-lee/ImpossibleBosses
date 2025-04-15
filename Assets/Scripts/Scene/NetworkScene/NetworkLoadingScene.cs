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
    public override Define.Scene CurrentScene => Define.Scene.NetworkLoadingScene;
    private NGO_LoadingScene_RPC_Caller _ngo_loadingScene_Rpc_Caller;

    private int _doneLoadScenePlayerCount = 0;
    public int DoneLoadScenePlayerCount
    {
        get => _doneLoadScenePlayerCount;
        set
        {
            _doneLoadScenePlayerCount = value;
        }
    }

    public NGO_LoadingScene_RPC_Caller NGO_Loading_RPC_Caller
    {
        get
        {
            if (_ngo_loadingScene_Rpc_Caller == null)
            {
                foreach (NetworkObject ngo in Managers.RelayManager.NetworkManagerEx.SpawnManager.SpawnedObjectsList)
                {
                    if (ngo.TryGetComponent(out NGO_LoadingScene_RPC_Caller ngo_LoadingScene_RPC_Caller))
                    {
                        _ngo_loadingScene_Rpc_Caller = ngo_LoadingScene_RPC_Caller;
                        break;
                    }
                }
            }
            return _ngo_loadingScene_Rpc_Caller;
        }
    }

    public override void Clear()
    {
    }


    protected override async void StartInit()
    {
        base.StartInit();
        Managers.SceneManagerEx.SetNextScene(Define.Scene.GamePlayScene);
        await LeaveLobbyAndVivox();
        HostSpawn();
        void HostSpawn()
        {
            Managers.RelayManager.SpawnNetworkOBJ("Prefabs/NGO/Scene_NGO/NGO_LoadingScene_RPC_Caller");
        }
        async Task LeaveLobbyAndVivox()
        {
            await Managers.LobbyManager.LeaveCurrentLobby();
            await Managers.VivoxManager.LogoutOfVivoxAsync();
        }
    }
    protected override void AwakeInit()
    {
        _ui_loding = Managers.UI_Manager.GetSceneUIFromResource<UI_Loading>();
    }

    //public IEnumerator LoadingSceneProcess()
    //{
    //    float processLength = 0.9f / Managers.RelayManager.CurrentUserCount;
    //    float pretimer = 0f;
    //    float aftertimer = 0f;


    //    while (DoneLoadScenePlayerCount != Managers.RelayManager.CurrentUserCount)
    //    {

    //    }


    //    //콜백받으면 채우고
    //}
}

//public IEnumerator LoadingSceneProcess(Action doneAction)
//{
//    float processLength = 0.9f / Managers.RelayManager.CurrentUserCount;
//    bool ischeckSendtoDoneLoaded = false;
//    float pretimer = 0f;
//    float aftertimer = 0f;
//    while (operation.isDone == false)
//    {
//        yield return null;

//        if (_ui_loding.LoaingSliderValue < 0.9f)
//        {
//            if (ischeckSendtoDoneLoaded == false) //나는 로딩이 다됐으니 서버에게 다 됐다고 호출
//            {
//                NGO_Loading_RPC_Caller.CalltoDoneLoadingRpc();
//                ischeckSendtoDoneLoaded = true;
//                Debug.Log("신호보냈슴");
//            }
//            _ui_loding.LoaingSliderValue = DoneLoadScenePlayerCount * processLength;
//            pretimer += Time.deltaTime / 5f;
//            _ui_loding.LoaingSliderValue = Mathf.Lerp(_ui_loding.LoaingSliderValue - processLength, _ui_loding.LoaingSliderValue + processLength, pretimer);
//        }
//        else
//        {
//            aftertimer += Time.deltaTime / 5f;
//            _ui_loding.LoaingSliderValue = Mathf.Lerp(0.9f, 1, aftertimer);
//            if (_ui_loding.LoaingSliderValue >= 1.0f)
//            {
//                operation.allowSceneActivation = true;
//                yield break;
//            }
//        }
//    }
//}
//}
//public IEnumerator LoadingSceneProcess(Action doneAction)
//{
//    AsyncOperation operation = SceneManager.LoadSceneAsync(Managers.SceneManagerEx.NextScene.ToString());
//    operation.allowSceneActivation = false;
//    while (_isCheckTaskChecker == null)
//    {
//        yield return null;
//    }
//    float pretimer = 0f;
//    float aftertimer = 0f;
//    float processLength = 0.9f / _isCheckTaskChecker.Length;

//    while (operation.isDone == false)
//    {
//        yield return null;

//        Debug.Log(operation.progress);

//        if (_ui_loding.LoaingSliderValue < 0.9f)
//        {
//            int sucessCount = 0;
//            foreach (bool OperationSucess in _isCheckTaskChecker)
//            {
//                if (OperationSucess is true)
//                {
//                    doneAction.Invoke();
//                    _isCheckTaskChecker[sucessCount] = true;
//                }
//            }
//            _ui_loding.LoaingSliderValue = sucessCount * processLength;
//            pretimer += Time.deltaTime / 5f;
//            _ui_loding.LoaingSliderValue = Mathf.Lerp(_ui_loding.LoaingSliderValue - processLength, _ui_loding.LoaingSliderValue + processLength, pretimer);
//        } 
//        else
//        {
//            aftertimer += Time.deltaTime / 5f;
//            _ui_loding.LoaingSliderValue = Mathf.Lerp(0.9f, 1, aftertimer);
//            if (_ui_loding.LoaingSliderValue >= 1.0f)
//            {
//                operation.allowSceneActivation = true;
//                yield break;
//            }
//        }
//    }
//}
