using System;
using Unity.Netcode;
using UnityEngine;

public class Module_NGO_GamePlaySceneSpawn : MonoBehaviour
{
    void Start()
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
