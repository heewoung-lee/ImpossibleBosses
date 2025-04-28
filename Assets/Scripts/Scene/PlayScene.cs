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

public class PlayScene : BaseScene
{
    public override Define.Scene CurrentScene => Define.Scene.GamePlayScene;

    private UI_Stage_Timer _ui_stage_timer;
    private UI_Loading _ui_Loading_Scene;
    private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;

    protected override void AwakeInit()
    {
    }
    protected override void StartInit()
    {
        base.StartInit();
        _ui_Loading_Scene = Managers.UI_Manager.GetOrCreateSceneUI<UI_Loading>();
        _gamePlaySceneLoadingProgress = _ui_Loading_Scene.AddComponent<GamePlaySceneLoadingProgress>();
        _ui_stage_timer = Managers.UI_Manager.GetOrCreateSceneUI<UI_Stage_Timer>();
    }

    public void Init_NGO_PlayScene_OnHost()
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost)
        {
            Managers.RelayManager.Load_NGO_Prefab<NGO_PlaySceneSpawn>();
            Managers.NGO_PoolManager.Create_NGO_Pooling_Object();
        }
    }

    public override void Clear()
    {
    }

}