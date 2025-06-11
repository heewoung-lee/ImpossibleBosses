using Controller;
using GameManagers;
using UnityEngine;

public class Module_Call_ToFollwingCamera : MonoBehaviour
{
    GameObject _player_Follwing_Camera;
    void Start()
    {
        _player_Follwing_Camera = GameObject.Find("PlayerFollowingCamera") == true ? 
            GameObject.Find("PlayerFollowingCamera") : Managers.ResourceManager.Instantiate("Prefabs/Camera/PlayerFollowingCamera");
        _player_Follwing_Camera.GetOrAddComponent<PlayerFollowingCamera>();
    }

}
