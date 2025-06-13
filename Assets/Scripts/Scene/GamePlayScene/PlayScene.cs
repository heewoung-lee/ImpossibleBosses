using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameManagers;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PlayScene : BaseScene,ISkillInit, ISceneController
{
    public override Define.Scene CurrentScene => Define.Scene.GamePlayScene;

    private UI_Loading _ui_Loading_Scene;
    private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;
    private MoveSceneController _sceneController;
    MoveSceneController ISceneController.SceneMoverController => _sceneController;

    [SerializeField] bool isTest = false;
    [SerializeField] bool isSoloTest = false;
    protected override void AwakeInit()
    {
        Managers.RelayManager.NetworkManagerEx.SceneManager.OnLoadEventCompleted += LoadPlayScene;
    }

    private void LoadPlayScene(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName != Define.Scene.GamePlayScene.ToString())
            return;

        _sceneController.InitGamePlayScene();
        _sceneController.SpawnOBJ();
    }

    protected override void StartInit()
    {
        base.StartInit();
        _ui_Loading_Scene = Managers.UIManager.GetOrCreateSceneUI<UI_Loading>();
       _gamePlaySceneLoadingProgress = _ui_Loading_Scene.AddComponent<GamePlaySceneLoadingProgress>();
        if (isTest == true)
        {
            _sceneController = new MoveSceneController(new MockUnitGamePlayScene(Define.PlayerClass.Fighter, _ui_Loading_Scene, isSoloTest));
            gameObject.AddComponent<MockUnit_UI_GamePlaySceneModule>();
        }
        else
        {
            _sceneController = new MoveSceneController(new UnitGamePlayScene());
            gameObject.AddComponent<MockUnit_UI_GamePlaySceneModule>();
            // gameObject.AddComponent<UI_GamePlaySceneModule>(); TODO: 빌드시 테스트 모듈 제거할것
        }

    }

   

    public override void Clear()
    {
    }

}