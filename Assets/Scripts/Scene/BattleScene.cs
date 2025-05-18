using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleScene : BaseScene
{
    private UI_Loading _ui_Loading_Scene;
    private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;

    public override Define.Scene CurrentScene => Define.Scene.BattleScene;

    protected override void StartInit()
    {
        base.StartInit();

        //Managers.UI_Manager.ShowUIPopupUI<Button_UI>();
        _gamePlaySceneLoadingProgress = _ui_Loading_Scene.AddComponent<GamePlaySceneLoadingProgress>();
        _ui_Loading_Scene = Managers.UI_Manager.GetOrCreateSceneUI<UI_Loading>();
        Init_NGO_PlayScene_OnHost();
    }
    public override void Clear()
    {

    }
    public void Init_NGO_PlayScene_OnHost()
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost)
        {
            Managers.RelayManager.Load_NGO_Prefab<NGO_PlaySceneSpawn>();
            Managers.NGO_PoolManager.Create_NGO_Pooling_Object();
        }
    }


    protected override void AwakeInit()
    {
        //_player = Managers.GameManagerEx.Spawn($"Prefabs/Player/{SpawnPlayerClass}");
        //_boss = Managers.GameManagerEx.Spawn("Prefabs/Enemy/Boss/Character/StoneGolem");

    }
}
