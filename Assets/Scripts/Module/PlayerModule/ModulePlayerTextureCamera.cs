using GameManagers;
using GameManagers.Interface.Resources_Interface;
using UnityEngine;
using Zenject;

namespace Module.PlayerModule
{
    public class ModulePlayerTextureCamera : MonoBehaviour
    {
        [Inject] private IInstantiate _instantiate;
        private GameObject _playerTextureCamara;
        void Start()
        {
            _playerTextureCamara = _instantiate.Instantiate("Prefabs/Player/PlayerInvenTextureCamera", transform);
        }
    }
}
