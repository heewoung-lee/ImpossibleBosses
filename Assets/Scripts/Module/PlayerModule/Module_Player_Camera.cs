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

    //TODO: 12.23�� �ó׸ӽ�ī�޶�� �ٲٱ����� �ش� ������Ʈ�� �÷��̾�� �и�
}
