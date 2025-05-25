using System;
using Unity.Netcode;
using UnityEngine;

public class Module_NGO_GamePlaySceneSpawn : MonoBehaviour
{
    void Start()
    {
        GetComponent<IGamePlaySceneSpawnBehaviour>().SpawnOBJ();
    }
}
