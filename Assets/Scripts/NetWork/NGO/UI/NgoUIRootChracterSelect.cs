using GameManagers;
using UI.Scene.SceneUI;
using Unity.Netcode;
using Zenject;

namespace NetWork.NGO.UI
{
    public class NgoUIRootChracterSelect : NetworkBehaviour
    {
        [Inject]private UIManager _uiManager; 
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsHost == false)
                return;

            transform.SetParent(Managers.RelayManager.NgoRootUI.transform);
            _uiManager.Get_Scene_UI<UIRoomCharacterSelect>().Set_NGO_UI_Root_Character_Select(this.transform);
        }
    }
}
