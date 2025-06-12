using GameManagers;
using UnityEngine;

namespace Module.PlayerModule
{
    public class ModulePlayerTextureCamera : MonoBehaviour
    {
        private GameObject _playerTextureCamara;
        void Start()
        {
            _playerTextureCamara = Managers.ResourceManager.Instantiate("Prefabs/Player/PlayerInvenTextureCamera", transform);
        }
    }
}
