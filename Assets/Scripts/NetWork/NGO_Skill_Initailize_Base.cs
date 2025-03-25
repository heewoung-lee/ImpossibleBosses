using Unity.Netcode;
using UnityEngine;

public abstract class NGO_Skill_Initailize_Base : NGO_InitailizeBase
{
    public abstract void SetTargetInitalze(NetworkObject targetNgo);


    public abstract void InvokeSkill();
}
