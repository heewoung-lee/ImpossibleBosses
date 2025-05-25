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
    private UI_ShowInteraction_ICON _iconUI;


    public UI_ShowInteraction_ICON IconUI
    {
        get
        {
            if (_iconUI == null)
            {
                _iconUI = Managers.UI_Manager.MakeUIWorldSpaceUI<UI_ShowInteraction_ICON>();
            }
            return _iconUI;
        }
    }

    private IInteraction _interactionTarget;
    private PlayerController _playerController;

    public IInteraction InteractionTarget { get { return _interactionTarget; } }
    public PlayerController PlayerController => _playerController;

    private void Awake()
    {
        _playerController = GetComponentInParent<PlayerController>();
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
        sphereCollider.radius = 1.2f; // ���� �ݰ�
        IconUI.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteraction interaction) && interaction.CanInteraction == true)
        {
            _interactionTarget = interaction;
            IconUI.transform.SetParent(Managers.UI_Manager.Root.transform);
            IconUI.gameObject.SetActive(true);
            IconUI.SetInteractionText(interaction.InteractionName, interaction.InteractionNameColor);
            IconUI.transform.position = new Vector3(other.transform.position.x, other.GetComponent<Collider>().bounds.max.y + Y_POSITION_OFFSET, other.transform.position.z);
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
            _interactionTarget.Interaction(this);
        }
    }

    public void DisEnable_Icon_UI()
    {
        IconUI.gameObject.SetActive(false);
        _interactionTarget = null;
    }
}
