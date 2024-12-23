using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

public class Module_Player_Camera : MonoBehaviour
{
    void Start()
    {
        Camera.main.GetOrAddComponent<CameraController>().SetPlayer(transform.gameObject);
    }

    //TODO: 12.23일 시네머신카메라로 바꾸기위해 해당 컴포넌트를 플레이어와 분리
}
