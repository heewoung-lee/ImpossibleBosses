using UnityEngine;

namespace Module.NGO_Module
{
    public class ModuleNgoGamePlaySceneSpawn : MonoBehaviour
    {
        void Start()
        {
            GetComponent<ISceneSpawnBehaviour>().SpawnOBJ();
        }
    }
}
