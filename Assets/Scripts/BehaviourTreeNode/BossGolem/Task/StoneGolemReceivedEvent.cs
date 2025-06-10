using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviourTreeNode.BossGolem.Task
{
    [TaskDescription("Returns success as soon as the event specified by eventName has been received.")]
    [TaskIcon("{SkinColor}HasReceivedEventIcon.png")]
    public class StoneGolemReceivedEvent : Conditional
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The name of the event to receive")]
        [SerializeField]private SharedString _eventName = "";

        private bool _eventReceived = false;
        private bool _registered = false;

        public override void OnStart()
        {
            // Let the behavior tree know that we are interested in receiving the event specified
            if (!_registered) {
                Owner.RegisterEvent(_eventName.Value, ReceivedEvent);
                _registered = true;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (_eventReceived)
            {
                Owner.EnableBehavior();
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }

        public override void OnEnd()
        {
            if (_eventReceived) {
                Owner.UnregisterEvent(_eventName.Value, ReceivedEvent);
                _registered = false;
            }
            _eventReceived = false;
        }

        private void ReceivedEvent()
        {
            _eventReceived = true;
        }
      

        public override void OnBehaviorComplete()
        {
            // Stop receiving the event when the behavior tree is complete
            Owner.UnregisterEvent(_eventName.Value, ReceivedEvent);
            _eventReceived = false;
            _registered = false;
        }

        public override void OnReset()
        {
            // Reset the properties back to their original values
            _eventName = "";
        }
    }
}