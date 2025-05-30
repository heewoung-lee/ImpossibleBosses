using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayScene : BaseScene,ISkillInit
{
    public override Define.Scene CurrentScene => Define.Scene.GamePlayScene;

    private UI_Loading _ui_Loading_Scene;
    private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;
    private PlaySceneController _playSceneController;
    [SerializeField] bool isTest = false;
    [SerializeField] bool isSoloTest = false;
    protected override void AwakeInit()
    {
    }
    protected override void StartInit()
    {
        base.StartInit();
        _ui_Loading_Scene = Managers.UI_Manager.GetOrCreateSceneUI<UI_Loading>();
       _gamePlaySceneLoadingProgress = _ui_Loading_Scene.AddComponent<GamePlaySceneLoadingProgress>();

        if (isTest == true)
        {
            _playSceneController = new PlaySceneController(new MockUnitGamePlayScene(Define.PlayerClass.Fighter, _ui_Loading_Scene, isSoloTest));
        }
        else
        {
            _playSceneController = new PlaySceneController(new UnitGamePlayScene());
        }
        _playSceneController.InitGamePlayScene();
        _playSceneController.SpawnOBJ();
    }

    public override void Clear()
    {
    }

}