using Controller.PlayerState.FighterState;
using GameManagers;
using Player;
using UnityEngine;
using Util;

namespace Module.PlayerModule.PlayerClassModule
{
    public class ModuleFighterClass : ModulePlayerClass
    {
        private const float DefalutTransitionRoar = 0.1f;
        private const float DefalutTransitionTaunt = 0.1f;
        private const float DefalutTransitionSlash = 0.3f;

        public override Define.PlayerClass PlayerClass => Define.PlayerClass.Fighter;

        private PlayerController _controller;
        private RoarState _roarState;
        private DeterminationState _determinationState;
        private SlashState _slashState;
        private TauntState _tauntState;


        private int _hashRoar = Animator.StringToHash("Roar");
        private int _hashSlash = Animator.StringToHash("Slash");
        public int HashSlash => _hashSlash;
        private int _hashTaunt = Animator.StringToHash("Taunt");


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
            _controller.StateAnimDict.RegisterState(_roarState, () => _controller.RunAnimation(_hashRoar, DefalutTransitionRoar));
            _controller.StateAnimDict.RegisterState(_determinationState, () => _controller.RunAnimation(_hashRoar, DefalutTransitionRoar));
            _controller.StateAnimDict.RegisterState(_slashState, () => _controller.RunAnimation(_hashSlash, DefalutTransitionSlash));
            _controller.StateAnimDict.RegisterState(_tauntState, () => _controller.RunAnimation(_hashTaunt, DefalutTransitionTaunt));
        }

        public void UpdateRoar()
        {
            _controller.ChangeAnimIfCurrentIsDone(_hashRoar, _controller.BaseIDleState);
        }

        public void UpdateSlash()
        {
            _controller.ChangeAnimIfCurrentIsDone(_hashSlash, _controller.BaseIDleState);
        }

        private void UpdateTaunt()
        {
            _controller.ChangeAnimIfCurrentIsDone(_hashTaunt, _controller.BaseIDleState);
        }

        private void UpdateDetermination()
        {
            _controller.ChangeAnimIfCurrentIsDone(_hashRoar, _controller.BaseIDleState);
        }
    }
}
