using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSkill
{
    public abstract Define.PlayerClass PlayerClass { get; }
    public abstract string SkillName { get; }
    public abstract float CoolTime {  get; }
    public abstract string EffectDescriptionText { get; }
    public abstract string ETCDescriptionText { get; }
    public abstract Sprite SkillconImage { get; }
    public abstract float Value { get; }
    public abstract void InvokeSkill();

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