using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.AI;

public class PlayScene : BaseScene
{
    public override Define.Scene CurrentScene => Define.Scene.GamePlayScene;

    private UI_Stage_Timer _ui_stage_timer;

    protected override void AwakeInit()
    {
    }
    protected override void StartInit()
    {
        base.StartInit();
        _ui_stage_timer = Managers.UI_Manager.GetOrCreateSceneUI<UI_Stage_Timer>();
        Init_NGO_PlayScene_OnHost();
    }

    private void Init_NGO_PlayScene_OnHost()
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost)
        {
            Managers.RelayManager.Load_NGO_Prefab<NGO_PlaySceneSpawn>();
            Managers.NGO_PoolManager.Create_NGO_Pooling_Object();//네트워크 오브젝트 풀링 생성
        }
    }

    public override void Clear()
    {
    }

}