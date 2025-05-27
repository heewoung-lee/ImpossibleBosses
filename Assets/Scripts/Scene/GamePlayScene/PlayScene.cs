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
    }
    protected override void StartInit()
    {
        base.StartInit();
        _ui_Loading_Scene = Managers.UI_Manager.GetOrCreateSceneUI<UI_Loading>();
       _gamePlaySceneLoadingProgress = _ui_Loading_Scene.AddComponent<GamePlaySceneLoadingProgress>();

        if (isTest == true)
        {
            _sceneController = new MoveSceneController(new MockUnitGamePlayScene(Define.PlayerClass.Fighter, _ui_Loading_Scene, isSoloTest));
        }
        else
        {
            _sceneController = new MoveSceneController(new UnitGamePlayScene());
        }
        _sceneController.InitGamePlayScene();
        _sceneController.SpawnOBJ();
        SetPlayerPosition();


        void SetPlayerPosition()
        {
            if(Managers.GameManagerEx.Player == null)
            {
                Managers.GameManagerEx.OnPlayerSpawnEvent += (PlayerStats) => { PlayerSpawnPosition(PlayerStats.GetComponent<NavMeshAgent>()); };
            }
            else
            {
                PlayerSpawnPosition(Managers.GameManagerEx.Player.GetComponent<NavMeshAgent>());
            }
        }
        void PlayerSpawnPosition(NavMeshAgent navMesh)
        {
            navMesh.Warp(new Vector3(Managers.GameManagerEx.Player.GetComponent<NetworkObject>().OwnerClientId, 0, 0));
        }
    }

   

    public override void Clear()
    {
    }

}