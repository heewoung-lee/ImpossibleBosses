using System;
using UnityEngine;

public class Module_Fighter_Class : Module_Player_Class
{
    private const float DEFALUT_TRANSITION_ROAR = 0.1f;
    private const float DEFALUT_TRANSITION_TAUNT = 0.1f;
    private const float DEFALUT_TRANSITION_Slash = 0.3f;

    public override Define.PlayerClass PlayerClass => Define.PlayerClass.Fighter;

    private PlayerController _controller;
    private RoarState _roarState;
    private DeterminationState _determinationState;
    private SlashState _slashState;
    private TauntState _tauntState;


    private int _hash_Roar = Animator.StringToHash("Roar");
    public int Hash_Roar => _hash_Roar;
    private int _hash_Slash = Animator.StringToHash("Slash");
    public int Hash_Slash => _hash_Slash;
    private int _hash_Taunt = Animator.StringToHash("Taunt");
    public int Hash_Taunt=> _hash_Taunt;


    public RoarState RoarState { get => _roarState; }
    public SlashState SlashState { get => _slashState; }
    public TauntState TauntState { get => _tauntState; }
    public DeterminationState DeterminationState { get => _determinationState; }

    public override void InitializeOnAwake()
    {
        base.InitializeOnAwake();
        _roarState = new RoarState(UpdateRoar);
        _determinationState = new DeterminationState(UpdateDetermination);
        _slashState = new SlashState(UpdateSlash);
        _tauntState = new TauntState(UpdateTaunt);
    }


    public override void InitializeOnStart()
    {
        base.InitializeOnStart();

        if(TryGetComponent(out PlayerController controller))
        {
            _controller = controller;
            InitailizeState();
        }
        else
        {
            Managers.SocketEventManager.DonePlayerSpawnEvent += Initailize_Player_DoneEvent;
        }

    }
    private void Initailize_Player_DoneEvent(GameObject player)
    {
        _controller = player.GetComponent<PlayerController>();
        InitailizeState();
    }

    private void InitailizeState()
    {
        _controller.StateAnimDict.RegisterState(_roarState, () => _controller.RunAnimation(_hash_Roar, DEFALUT_TRANSITION_ROAR));
        _controller.StateAnimDict.RegisterState(_determinationState, () => _controller.RunAnimation(_hash_Roar, DEFALUT_TRANSITION_ROAR));
        _controller.StateAnimDict.RegisterState(_slashState, () => _controller.RunAnimation(_hash_Slash, DEFALUT_TRANSITION_Slash));
        _controller.StateAnimDict.RegisterState(_tauntState, () => _controller.RunAnimation(_hash_Taunt, DEFALUT_TRANSITION_TAUNT));
    }

    public void UpdateRoar()
    {
        _controller.ChangeAnimIfCurrentIsDone(_hash_Roar, _controller.Base_IDleState);
    }

    public void UpdateSlash()
    {
        _controller.ChangeAnimIfCurrentIsDone(_hash_Slash, _controller.Base_IDleState);
    }

    private void UpdateTaunt()
    {
        _controller.ChangeAnimIfCurrentIsDone(_hash_Taunt, _controller.Base_IDleState);
    }

    private void UpdateDetermination()
    {
        _controller.ChangeAnimIfCurrentIsDone(_hash_Roar, _controller.Base_IDleState);
    }
}
