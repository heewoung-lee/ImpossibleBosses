using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// �������� �������� ����ϴ� �ݶ��̾�� ���� ��ġ�� ��ġ�� ������,
/// �������� ����ϴ� �ݶ��̾�� ���ļ� 2��� ���� ������ �߻��߰�
/// ������Ʈ�� ����
/// </summary>
public class Module_Player_Interaction : MonoBehaviour 
{
    private const float Y_POSITION_OFFSET = 0.2f;

    private InputAction _interactionInput;
    private UI_ShowInteraction_ICON _IconUI;
    private IInteraction _interactionTarget;

    private void Awake()
    {
        _interactionInput = Managers.InputManager.GetInputAction(Define.ControllerType.Player, "Interaction");
        _interactionInput.Enable();
    }

    private void OnEnable()
    {
        _interactionInput.performed += Interaction;
    }

    private void OnDisable()
    {
        _interactionInput.performed -= Interaction;
    }

    void Start()
    {
        SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = 1.5f; // ���� �ݰ�
        _IconUI = Managers.UI_Manager.MakeUIWorldSpaceUI<UI_ShowInteraction_ICON>();
        _IconUI.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteraction interaction) && interaction.CanInteraction == true)
        {
            _interactionTarget = interaction;
            _IconUI.transform.SetParent(Managers.UI_Manager.Root.transform);
            _IconUI.gameObject.SetActive(true);
            _IconUI.SetInteractionText(interaction.InteractionName, interaction.InteractionNameColor);
            _IconUI.transform.position = new Vector3(other.transform.position.x, other.GetComponent<Collider>().bounds.max.y + Y_POSITION_OFFSET, other.transform.position.z);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteraction interaction))
        {
            interaction.OutInteraction();
            DisEnable_Icon_UI();
        }
    }
    public void Interaction(InputAction.CallbackContext context)
    {
        if (_interactionTarget != null)
        {
            _interactionTarget.Interaction(transform);
        }
    }

    public void DisEnable_Icon_UI()
    {
        _IconUI.gameObject.SetActive(false);
        _interactionTarget = null;
    }
}
