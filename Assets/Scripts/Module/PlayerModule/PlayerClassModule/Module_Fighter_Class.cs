using UnityEngine;

public class Module_Fighter_Class : Module_Player_Class
{
    private const float DEFALUT_TRANSITION_ROAR = 0.1f;
    private const float DEFALUT_TRANSITION_Slash = 0.3f;

    public override Define.PlayerClass PlayerClass => Define.PlayerClass.Fighter;

    private PlayerController _controller;
    private RoarState _roarState;
    private SlashState _slashState;
    private int _hash_Roar => Animator.StringToHash("Roar");
    private int _hash_Slash => Animator.StringToHash("Slash");


    public RoarState RoarState { get => _roarState; }
    public SlashState SlashState { get => _slashState; }


    public override void InitAwake()
    {
        base.InitAwake();
        _roarState = new RoarState(UpdateRoar);
        _slashState = new SlashState(UpdateSlash);
    }

    public override void InitStart()
    {
        base.InitStart();
        _controller = GetComponent<PlayerController>();
        _controller.StateAnimDict.RegisterState(_roarState,()=>_controller.RunAnimation(_hash_Roar, DEFALUT_TRANSITION_ROAR));
        _controller.StateAnimDict.RegisterState(_slashState,()=>_controller.RunAnimation(_hash_Slash,DEFALUT_TRANSITION_Slash));
    }


    public void UpdateRoar()
    {
        _controller.ChangeAnimIfCurrentIsDone(_hash_Roar, _controller.Base_IDleState);
    }

    public void UpdateSlash()
    {
        _controller.ChangeAnimIfCurrentIsDone(_hash_Slash, _controller.Base_IDleState);
    }
}
