using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Returns success as soon as the event specified by eventName has been received.")]
    [TaskIcon("{SkinColor}HasReceivedEventIcon.png")]
    public class StoneGolemReceivedEvent : Conditional
    {
        [Tooltip("The name of the event to receive")]
        public SharedString eventName = "";

        private bool eventReceived = false;
        private bool registered = false;
        private BaseController owner_Controller;



        public override void OnStart()
        {
            // Let the behavior tree know that we are interested in receiving the event specified
            if (!registered) {
                Owner.RegisterEvent(eventName.Value, ReceivedEvent);
                registered = true;
            }

            owner_Controller = Owner.GetComponent<BaseController>();
        }

        public override TaskStatus OnUpdate()
        {
            if (eventReceived)
            {
                Owner.EnableBehavior();
                owner_Controller.CurrentStateType = owner_Controller.Base_DieState;
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }

        public override void OnEnd()
        {
            if (eventReceived) {
                Owner.UnregisterEvent(eventName.Value, ReceivedEvent);
                registered = false;
            }
            eventReceived = false;
        }

        private void ReceivedEvent()
        {
            eventReceived = true;
        }
      

        public override void OnBehaviorComplete()
        {
            // Stop receiving the event when the behavior tree is complete
            Owner.UnregisterEvent(eventName.Value, ReceivedEvent);
            eventReceived = false;
            registered = false;
        }

        public override void OnReset()
        {
            // Reset the properties back to their original values
            eventName = "";
        }
    }
}