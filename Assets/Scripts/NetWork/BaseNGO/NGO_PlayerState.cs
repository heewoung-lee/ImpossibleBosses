using UnityEngine;

public class NGO_PlayerState : NetworkBehaviourBase
{
    private IState _currentStateType;
    private IState CurrentStateType => _currentStateType;

    protected override void AwakeInit()
    {
        if(IsOwner == true)
        {
            _currentStateType = GetComponent<BaseController>().CurrentStateType;
        }
    }
    protected override void StartInit()
    {
    }

}
