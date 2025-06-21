using GameManagers;
using GameManagers.Interface;
using GameManagers.Interface.InputManager_Interface;
using GameManagers.Interface.UI_Interface;
using Module.CommonModule;
using Player;
using UI.WorldSpace;
using UnityEngine;
using UnityEngine.InputSystem;
using Util;
using Zenject;

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
        private UIShowInteractionIcon _iconUI;
        [Inject] private IUIManager _uiManager;
        [Inject] private IUISubItem _subItemUIManager;
        [Inject] private IInputAsset _inputManager;

        public UIShowInteractionIcon IconUI
        {
            get
            {
                if (_iconUI == null)
                {
                    _iconUI = _subItemUIManager.MakeUIWorldSpaceUI<UIShowInteractionIcon>();
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
            _interactionInput = _inputManager.GetInputAction(Define.ControllerType.Player, "Interaction");
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
                IconUI.transform.SetParent(_uiManager.Root.transform);
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
