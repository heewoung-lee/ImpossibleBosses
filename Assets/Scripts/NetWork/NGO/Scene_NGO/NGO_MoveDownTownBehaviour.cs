using GameManagers;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NGO_MoveDownTownBehaviour : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(Managers.UIManager.Try_Get_Scene_UI(out UI_Boss_HP bossHP))
        {
            Managers.ResourceManager.DestroyObject(bossHP.gameObject);
        }
    }
}
