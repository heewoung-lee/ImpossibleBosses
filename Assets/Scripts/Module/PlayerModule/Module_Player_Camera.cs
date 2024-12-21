using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Module_Player_Camera : MonoBehaviour
{
    void Start()
    {
        Camera.main.GetOrAddComponent<CameraController>().SetPlayer(transform.gameObject);
    }
}
