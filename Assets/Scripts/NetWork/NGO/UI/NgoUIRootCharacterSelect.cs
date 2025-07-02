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
        public class NgoUIRootCharacterSelectFactory : IFactory<NgoUIRootCharacterSelect>
        {
            private readonly DiContainer _container;
            public NgoUIRootCharacterSelectFactory(DiContainer container)
            {
                _container = container;
            }
            public NgoUIRootCharacterSelect Create()
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/NGO/NGOUIRootChracterSelect");
                GameObject gameObject = Instantiate(prefab);
                
                _container.InjectGameObject(gameObject);
                
                return gameObject.GetComponent<NgoUIRootCharacterSelect>();
            }
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
