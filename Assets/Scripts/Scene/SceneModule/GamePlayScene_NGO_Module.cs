using UnityEngine;

public class GamePlayScene_NGO_Module : MonoBehaviour
{
    protected void Start()
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost)
            Managers.RelayManager.Load_NGO_ROOT_UI_Module("NGO/PlayerSpawner");
    }
}
