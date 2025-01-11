using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleScene : BaseScene
{
    GameObject _player;
    GameObject _boss;


    public Define.PlayerClass SpawnPlayerClass;

    public override Define.Scene CurrentScene => Define.Scene.BattleScene;

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
        _player = Managers.GameManagerEx.Spawn($"Prefabs/Player/{SpawnPlayerClass}");
        _boss = Managers.GameManagerEx.Spawn("Prefabs/Enemy/Boss/Character/StoneGolem");

    }
}
