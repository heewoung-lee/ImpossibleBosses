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
        [Inject]private IUISceneManager _uiSceneManager; 
        [Inject] private RelayManager _relayManager;
        
        public class NgoUIRootCharacterSelectFactory : NgoZenjectFactory<NgoUIRootCharacterSelect>
        {
            public NgoUIRootCharacterSelectFactory(DiContainer container, GameObject ngo)
            {
                _container = container;
                _ngo = ngo;
            }
        }

        
        

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
