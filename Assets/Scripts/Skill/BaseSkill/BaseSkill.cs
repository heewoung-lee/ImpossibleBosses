using System.Collections.Generic;
using Controller;
using Controller.ControllerStats;
using GameManagers;
using UnityEngine;
using static UnityEngine.CullingGroup;

public abstract class BaseSkill
{
    public abstract Define.PlayerClass PlayerClass { get; }
    public abstract string SkillName { get; }
    public abstract float CoolTime {  get; }
    public abstract string EffectDescriptionText { get; }
    public abstract string ETCDescriptionText { get; }
    public abstract Sprite SkillconImage { get; }
    public abstract float Value { get; }

    public virtual void AddInitailzeState() { }
    public abstract BaseController PlayerController { get; protected set; }
    public abstract Module_Player_Class Module_Player_Class { get; protected set; }

    public abstract void SkillAction();

    public abstract IState state { get; }


    public virtual void InvokeSkill()
    {
        if (PlayerController == null || Module_Player_Class == null)
        {
            PlayerController = Managers.GameManagerEx.Player.GetComponent<BaseController>();
            Module_Player_Class = PlayerController.GetComponent<Module_Player_Class>();
            PlayerController.StateAnimDict.RegisterState(state, SkillAction);
            AddInitailzeState();
        }
        PlayerController.CurrentStateType = state;
    }

    private BaseController baseController;

    public bool IsStateUpdatedAfterSkill()
    {
        if(baseController == null)
        {
            baseController =  Managers.GameManagerEx.Player.GetComponent<BaseController>();
        }
        IState currentIState = baseController.CurrentStateType;
        InvokeSkill();

        return currentIState != baseController.CurrentStateType ? true : false;
    }

}