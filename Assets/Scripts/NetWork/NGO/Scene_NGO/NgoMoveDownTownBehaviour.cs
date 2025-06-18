using GameManagers;
using Unity.Netcode;
using Zenject;

namespace NetWork.NGO.Scene_NGO
{
    public class NgoMoveDownTownBehaviour : NetworkBehaviour
    {
        [Inject] private UIManager _uiManager;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if(_uiManager.Try_Get_Scene_UI(out UI_Boss_HP bossHp))
            {
                Managers.ResourceManager.DestroyObject(bossHp.gameObject);
            }
        }
    }
}
