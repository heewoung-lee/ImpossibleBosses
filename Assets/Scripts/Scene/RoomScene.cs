using System.Threading.Tasks;
using GameManagers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
      
        UI_Room_CharacterSelect uI_CharacterSelect = Managers.UIManager.GetSceneUIFromResource<UI_Room_CharacterSelect>();
        UI_RoomChat ui_Chatting = Managers.UIManager.GetSceneUIFromResource<UI_RoomChat>();
    }
}
