using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleScene : BaseScene, ISkillInit
{
    private UI_Loading _ui_Loading_Scene;
    private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;
    private MoveSceneController _battleSceneController;

    public bool isTest = false;
    public bool isSoloTest = false;



    public override Define.Scene CurrentScene => Define.Scene.BattleScene;

    protected override void StartInit()
    {
        base.StartInit();
        _ui_Loading_Scene = Managers.UI_Manager.GetOrCreateSceneUI<UI_Loading>();
        _gamePlaySceneLoadingProgress = _ui_Loading_Scene.AddComponent<GamePlaySceneLoadingProgress>();
        if (isTest == true)
        {
            _battleSceneController = new MoveSceneController(new MockUnitBattleScene(Define.PlayerClass.Fighter, _ui_Loading_Scene, isSoloTest));
        }
        else
        {
            _battleSceneController = new MoveSceneController(new UnitBattleScene());
        }
        _battleSceneController.InitGamePlayScene();
        //_battleSceneController.SpawnOBJ();
        _gamePlaySceneLoadingProgress.OnLoadingComplete += SpawnOBJ;

        void SpawnOBJ()
        {
            if (Managers.RelayManager.NetworkManagerEx.IsHost == false)
                return;

            _battleSceneController.SpawnOBJ();
        }
    }
    public override void Clear()
    {
    }
    protected override void AwakeInit()
    {
    }
}
