using GameManagers;
using GameManagers.Interface;
using GameManagers.Interface.Resources_Interface;
using GameManagers.Interface.ResourcesManager;
using GameManagers.Interface.UI_Interface;
using GameManagers.Interface.UIManager;
using UI.Scene.SceneUI;
using Unity.Netcode;
using Zenject;

namespace NetWork.NGO.Scene_NGO
{
    public class NgoMoveDownTownBehaviour : NetworkBehaviour
    {
        [Inject] private IUISceneManager _uiManager;
        [Inject] IDestroyObject _destroyer;
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if(_uiManager.Try_Get_Scene_UI(out UIBossHp bossHp))
            {
                _destroyer.DestroyObject(bossHp.gameObject);
            }
        }
    }
}
