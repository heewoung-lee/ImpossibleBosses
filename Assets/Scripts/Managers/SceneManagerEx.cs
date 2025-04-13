using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx:IManagerIResettable,IManagerInitializable
{ 
    private Define.Scene _currentSceneName;
    private LoadingScene _loadingScene;
    private Define.Scene _nextSceneName;

    public BaseScene GetCurrentScene { get => GameObject.FindAnyObjectByType<BaseScene>(); }
    public Define.Scene CurrentSceneName => _currentSceneName;
    public Define.Scene NextSceneName => _nextSceneName;

    public void SetNextSceneName(string nextScenename)
    {
        _nextSceneName = (Define.Scene)Enum.Parse(typeof(Define.Scene), nextScenename);
    }

    public void LoadScene(Define.Scene scene)
    {
        Managers.Clear();
        _currentSceneName = scene;
        SceneManager.LoadScene(GetEnumName(scene));
    }

    public void LoadSceneWithLoadingScreen(Define.Scene Nextscene)
    {
        LoadingScene.LoadNextScene(Nextscene);
        LoadScene(Define.Scene.LoadingScene);
    }

    public async Task NetworkLoadSceneAsync(Define.Scene scene)
    {
        //Managers.Clear();
        _currentSceneName = scene;
        await Managers.LobbyManager.LeaveCurrentLobby();
        Managers.RelayManager.NetworkManagerEx.SceneManager.LoadScene(GetEnumName(scene), UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public async Task NetworkLoadSceneAsyncWithLoadingScene(Define.Scene scene)
    {
        Managers.Clear();
        _currentSceneName = scene;
        await Managers.LobbyManager.LeaveCurrentLobby();
        await Managers.VivoxManager.LogoutOfVivoxAsync();//TODO: ���߿� �񺹽� �� �� ������ �� �κ� ���ٰ�

        //Managers.
        LoadingScene.LoadNextScene(_currentSceneName);
        //���⿡ RPC�ݷ��� ��� Ŭ���̾�Ʈ�� ������ ����

        
        Managers.RelayManager.NetworkManagerEx.SceneManager.LoadScene(Define.Scene.LoadingScene.ToString(), UnityEngine.SceneManagement.LoadSceneMode.Single);
        Managers.RelayManager.NetworkManagerEx.SceneManager.LoadScene(GetEnumName(scene), UnityEngine.SceneManagement.LoadSceneMode.Additive);

        //���� ��� Ŭ���̾�Ʈ�� �ε����� �ε� �ǰ� �� ����
        //�ε��� ������ 
    }


    public string GetEnumName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }

    public void Clear()
    {
        GetCurrentScene.Clear();
        Managers.UI_Manager.Clear();
    }

    public void Init()
    {
    }
}
