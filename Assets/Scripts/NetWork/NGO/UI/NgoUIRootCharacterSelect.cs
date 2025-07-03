using GameManagers;
using GameManagers.Interface.UIManager;
using UI.Scene.SceneUI;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace NetWork.NGO.UI
{
    public class NgoUIRootCharacterSelect : NetworkBehaviour
    {
        public class NgoUIRootCharacterSelectFactory : NgoZenjectFactory<NgoUIRootCharacterSelect>
        {
            public NgoUIRootCharacterSelectFactory(DiContainer container)
            {
                _container = container;
            }

            protected override string Path => "Prefabs/NGO/NGOUIRootChracterSelect";
        }
        
        [Inject]private IUISceneManager _uiSceneManager; 
        [Inject] private RelayManager _relayManager;
        
        

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsHost == false)
                return;

            transform.SetParent(_relayManager.NgoRootUI.transform);
            _uiSceneManager.Get_Scene_UI<UIRoomCharacterSelect>().Set_NGO_UI_Root_Character_Select(this.transform);
        }
    }
}
