using UnityEngine;

public class Module_Fighter_Class : Module_Player_Class
{
    private const float DEFALUT_TRANSITION_ROAR = 0.1f;
    public override Define.PlayerClass PlayerClass => Define.PlayerClass.Fighter;

    private PlayerController _controller;
    private RoarState _roarState;
    private int _hash_Roar => Animator.StringToHash("Roar");

    public RoarState RoarState { get => _roarState; }

    public override void InitAwake()
    {
        base.InitAwake();
        _roarState = new RoarState(UpdateRoar);
    }

    public override void InitStart()
    {
        base.InitStart();
        _controller = GetComponent<PlayerController>();
        _controller.StateAnimDict.RegisterState(_roarState,()=>_controller.RunAnimation(_hash_Roar, DEFALUT_TRANSITION_ROAR));
    }


    public void UpdateRoar()
    {
        _controller.ChangeStateToIdle("Roar");
    }
}
