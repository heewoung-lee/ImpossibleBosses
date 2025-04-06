using UnityEngine;

public class GamePlayScene_NGO_Module : MonoBehaviour
{
    protected void Start()
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost)
            Managers.RelayManager.Load_NGO_Prefab<NGO_PlaySceneSpawn>();
    }
}
