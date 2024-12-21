using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_PlayerRenderTextureCamara : MonoBehaviour
{
    private Transform _player; // �÷��̾� Transform

    void Start()
    {
        _player = GetComponentInParent<PlayerController>().transform;
    }

    void LateUpdate()
    {
        // ī�޶��� ��ġ�� �÷��̾� �������� ����
        // (�÷��̾��� ���鿡�� �ణ ���ʿ��� �ٶ󺸴� ��ġ�� ����)
        Vector3 cameraOffset = new Vector3(0f, 1.3f, -3f); // ���鿡�� �ణ ��
        transform.position = _player.position - _player.forward * cameraOffset.z + Vector3.up * cameraOffset.y;

        // �÷��̾ �ٶ󺸵��� ����
        transform.LookAt(_player.position + Vector3.up * 1.3f); // �÷��̾��� �Ӹ� ���̷� ����

        // �ʿ��ϸ� �߰����� ȸ�� ����
        transform.rotation *= Quaternion.Euler(15f, 0f, 0f); // ���ʿ��� �ٶ󺸴� ���� �߰�
    }
}
