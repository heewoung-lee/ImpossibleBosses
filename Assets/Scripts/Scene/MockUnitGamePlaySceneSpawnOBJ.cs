using UnityEngine;

public class MockUnitGamePlaySceneSpawnOBJ : MonoBehaviour, IGamePlaySceneSpawnBehaviour
{
    public void SpawnOBJ()
    {
        Managers.RelayManager.NetworkManagerEx.OnServerStarted += Init_NGO_PlayScene_OnHost;
        void Init_NGO_PlayScene_OnHost()
        {
            if (Managers.RelayManager.NetworkManagerEx.IsHost)
            {
                Managers.RelayManager.Load_NGO_Prefab<NGO_GamePlaySceneSpawn>();
            }
        }
    }

}
