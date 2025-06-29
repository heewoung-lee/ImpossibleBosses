using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BattleScene : BaseScene, ISkillInit, ISceneController
{
    private UI_Loading _ui_Loading_Scene;
    private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;
    private MoveSceneController _battleSceneController;

    public bool isTest = false;
    public bool isSoloTest = false;
    public override Define.Scene CurrentScene => Define.Scene.BattleScene;
    public MoveSceneController SceneMoverController => _battleSceneController;

    protected override void StartInit()
    {
        base.StartInit();
        _ui_Loading_Scene = Managers.UI_Manager.GetOrCreateSceneUI<UI_Loading>();
        _gamePlaySceneLoadingProgress = _ui_Loading_Scene.AddComponent<GamePlaySceneLoadingProgress>();
        if (isTest == true)
        {
            _battleSceneController = new MoveSceneController(new MockUnitBattleScene(Define.PlayerClass.Fighter, _ui_Loading_Scene, isSoloTest));
            gameObject.AddComponent<MockUnit_UI_GamePlaySceneModule>();
            _battleSceneController.InitGamePlayScene();
            _battleSceneController.SpawnOBJ();
        }
        else
        {
            _battleSceneController = new MoveSceneController(new UnitBattleScene());
            gameObject.AddComponent<MockUnit_UI_GamePlaySceneModule>();
            _battleSceneController.InitGamePlayScene();
            _gamePlaySceneLoadingProgress.OnLoadingComplete += _battleSceneController.SpawnOBJ;
        }
    }
    public override void Clear()
    {
    }
    protected override void AwakeInit()
    {
    }
}
