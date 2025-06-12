using GameManagers;
using Unity.Netcode;

namespace NetWork.NGO.Scene_NGO
{
    public class NgoMoveDownTownBehaviour : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if(Managers.UIManager.Try_Get_Scene_UI(out UI_Boss_HP bossHp))
            {
                Managers.ResourceManager.DestroyObject(bossHp.gameObject);
            }
        }
    }
}
