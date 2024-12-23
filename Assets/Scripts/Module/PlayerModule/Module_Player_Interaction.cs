using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Module_Player_Interaction : MonoBehaviour
{
    private const float Y_POSITION_OFFSET = 0.2f;


    private UI_Base _IconUI;
    private IInteraction _interactionTarget;
    void Start()
    {
        SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = 1.5f; // 감지 반경
        _IconUI = Managers.UI_Manager.MakeUIWorldSpaceUI<UI_ShowInteraction_ICON>();
        _IconUI.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteraction interaction))
        {
            _interactionTarget = interaction;
            _IconUI.transform.SetParent(other.transform);
            _IconUI.gameObject.SetActive(true);
            _IconUI.transform.position = new Vector3(other.transform.position.x, other.GetComponent<Collider>().bounds.max.y+Y_POSITION_OFFSET, other.transform.position.z);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _interactionTarget.OutInteraction();
        _IconUI.gameObject.SetActive(false);
        _interactionTarget = null;
    }

    private void Update()
    {
        //아이콘 E 보이기
        if (_interactionTarget != null && Input.GetKeyDown(KeyCode.E))
        {
            _interactionTarget.Interaction();
        }
    }
}
