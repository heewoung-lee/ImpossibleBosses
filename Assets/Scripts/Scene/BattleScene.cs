using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleScene : BaseScene
{
    GameObject _player;
    GameObject _boss;


    public Define.PlayerClass SpawnPlayerClass;

    protected override void StartInit()
    {
        base.StartInit();

        _player.transform.position = Vector3.zero + Vector3.left;
        //Managers.UI_Manager.ShowUIPopupUI<Button_UI>();

    }
    public override void Clear()
    {

    }
    protected override void AwakeInit()
    {
        currentScene = Define.Scene.Battle;
        _player = Managers.GameManagerEx.Spawn($"Prefabs/Player/{SpawnPlayerClass}");
        _boss = Managers.GameManagerEx.Spawn("Prefabs/Enemy/Boss/Character/StoneGolem");

    }
}
