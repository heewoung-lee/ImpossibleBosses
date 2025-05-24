using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleScene : BaseScene,ISkillInit
{
    private UI_Loading _ui_Loading_Scene;
    private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;
    private BattleSceneController _battleSceneController;

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
            _battleSceneController = new BattleSceneController(new MockUnitBattleScene(Define.PlayerClass.Fighter, _ui_Loading_Scene, isSoloTest));
        }
        else
        {
            _battleSceneController = new BattleSceneController(new UnitBattleScene());
        }
        _battleSceneController.Init();
        _battleSceneController.SpawnOBJ();
    }
    public override void Clear()
    {

    }
    public void Init_NGO_PlayScene_OnHost()
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost)
        {
            Managers.RelayManager.Load_NGO_Prefab<NGO_GamePlaySceneSpawn>();
        }
    }


    protected override void AwakeInit()
    {
        //_player = Managers.GameManagerEx.Spawn($"Prefabs/Player/{SpawnPlayerClass}");
        //_boss = Managers.GameManagerEx.Spawn("Prefabs/Enemy/Boss/Character/StoneGolem");

    }
}
