using UnityEngine;

public class GamePlaySceneSpawnOBJ : MonoBehaviour, IGamePlaySceneSpawnBehaviour
{
    public void SpawnOBJ()
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost)
        {
            Managers.RelayManager.Load_NGO_Prefab<NGO_GamePlaySceneSpawn>();
        }
    }
}
