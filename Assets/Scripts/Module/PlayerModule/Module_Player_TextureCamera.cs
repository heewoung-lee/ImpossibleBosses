using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_Player_TextureCamera : MonoBehaviour
{
    private GameObject _playerTextureCamara;
    void Start()
    {
        _playerTextureCamara = Managers.ResourceManager.Instantiate("Prefabs/Player/PlayerInvenTextureCamera", transform);
    }
}
