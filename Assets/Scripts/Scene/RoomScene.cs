using GameManagers;

namespace Scene
{
    public class RoomScene : BaseScene
    {
        public override Define.Scene CurrentScene => Define.Scene.RoomScene;

        public override void Clear()
        {
        }

        protected override void AwakeInit()
        {
        }

        protected override void StartInit()
        {
            base.StartInit();
      
            UI_Room_CharacterSelect uICharacterSelect = Managers.UIManager.GetSceneUIFromResource<UI_Room_CharacterSelect>();
            UI_RoomChat uiChatting = Managers.UIManager.GetSceneUIFromResource<UI_RoomChat>();
        }
    }
}
