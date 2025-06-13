using GameManagers;
using Module.CommonModule;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Module.PlayerModule
{
    /// <summary>
    /// 기존에는 데미지를 계산하는 콜라이어와 같은 위치에 배치를 했지만,
    /// 데미지를 계산하는 콜라이어와 겹쳐서 2배로 들어가는 문제가 발생했고
    /// 오브젝트를 나눔
    /// </summary>
    public class ModulePlayerInteraction : MonoBehaviour 
    {
        private const float YPositionOffset = 0.2f;
        private InputAction _interactionInput;
        private UI_ShowInteraction_ICON _iconUI;


        public UI_ShowInteraction_ICON IconUI
        {
            get
            {
                if (_iconUI == null)
                {
                    _iconUI = Managers.UIManager.MakeUIWorldSpaceUI<UI_ShowInteraction_ICON>();
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
            sphereCollider.radius = 1.2f; // 감지 반경
            IconUI.gameObject.SetActive(false);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IInteraction interaction) && interaction.CanInteraction == true)
            {
                _interactionTarget = interaction;
                IconUI.transform.SetParent(Managers.UIManager.Root.transform);
                IconUI.gameObject.SetActive(true);
                IconUI.SetInteractionText(interaction.InteractionName, interaction.InteractionNameColor);
                IconUI.transform.position = new Vector3(other.transform.position.x, other.GetComponent<Collider>().bounds.max.y + YPositionOffset, other.transform.position.z);
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
}
