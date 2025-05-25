using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public static readonly Vector3 DEFAULT_QUARTERVIEW_POSITION = new Vector3(0, 7, -6);
    public static readonly Quaternion DEFAULT_QUARTERVIEW_ROTATION = Quaternion.Euler(new Vector3(45, 0, 0));
    public const string APPLICATIONNAME = "ItemDataSheet";

    public enum PlayerClass
    {
        Archer,
        Fighter,
        Mage,
        Monk,
        Necromancer,
    }
    public enum ControllerLayer
    {
        Player,
        AnotherPlayer,
        Moster
    }
    public enum WorldObject
    {
        Unknown,
        Player,
        Monster,
        Boss
    }
    public enum Scene
    {
        Unknown,
        LoginScene,
        LobbyScene,
        RoomScene,
        GamePlayScene,
        BattleScene,
        LoadingScene,
        NetworkLoadingScene
    }
    public enum CurrentMouseType
    {
        None,
        Base,
        Attack,
        Talk
    }
    public enum Layer
    {
        Block = 8,
        Monster = 9,
        Npc = 10
    }
    public enum Sound
    {
        SFX,
        BGM
    }
    public enum UI_Event
    {
        LeftClick,
        RightClick,
        DragBegin,
        Drag,
        DragEnd,
        PointerEnter,
        PointerExit
    }
    public enum CameraMode
    {
        QuarterView,
    }

    public enum BossID
    {
        Golem = 101,
        Unknown1,
        Unknown2,
    }
    public enum MonsterID
    {
        Slime = 1,
        Unknown1,
        Unknown2,
    }
    public enum ControllerType
    {
        Player,
        Camera,
        UI
    }
}
