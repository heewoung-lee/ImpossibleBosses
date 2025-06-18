using UnityEngine;

namespace UI.Scene.SceneUI
{
    public class UIRoomInventory : UIScene
    {    enum Transforms
        {
            RoomContent
        }

        private Transform _roomContent;
        public Transform RoomContent => _roomContent;
        protected override void AwakeInit()
        {
            base.AwakeInit();
            Bind<Transform>(typeof(Transforms));
            _roomContent = Get<Transform>((int)Transforms.RoomContent);

        }

        protected override void StartInit()
        {
            base.StartInit();
        }
    }
}
