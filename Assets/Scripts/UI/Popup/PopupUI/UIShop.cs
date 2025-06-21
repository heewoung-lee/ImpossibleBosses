using System.Collections;
using System.Collections.Generic;
using Data.DataType.ItemType;
using Data.DataType.ItemType.Interface;
using Data.Item;
using GameManagers;
using GameManagers.Interface;
using GameManagers.Interface.UI_Interface;
using Stats;
using TMPro;
using UI.Scene.SceneUI;
using UI.SubItem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Util;
using Zenject;

namespace UI.Popup.PopupUI
{
    public class UIShop : UIPopup
    {
        private readonly string _focusTabColorHexcode = "#F6E19C";
        private readonly string _nonfocusTabColorHexcode = "#BEB5B6";
        enum ItemShopText
        {
            EquipItemTapText,
            ConsumableItemTapText,
            ETCItemTapText,
            PlayerHasGoldText
        }

        enum IconImages
        {
            EquipItemTap,
            ConsumableItemTap,
            ETCItemTap,
            TabFocusLine
        }


        Dictionary<GameObject, (TMP_Text, ItemType)> _findGameObjectToTMPTextDict;

        private TMP_Text _equipItemTapText;
        private TMP_Text _consumableItemTapText;
        private TMP_Text _etcItemTapText;
        private TMP_Text _playerHasGoldText;
        private TMP_Text _currentFocusText;

        private Image _equipItemIcon;
        private Image _consumableItemIcon;
        private Image _etcItemIcon;
        private Image _tabFocusLine;

        private Transform _itemCoordinate;
        private PlayerStats _playerStats;
        private Coroutine _moveCoroutine;

        private Color _focusColor;
        private Color _nonFocusColor;

        private GraphicRaycaster _uiShopRaycaster;
        private EventSystem _eventSystem;

        [Inject] private IUIPopupManager _popupUIManager;
        [Inject] private IUISceneManager _sceneUIManager;
        [Inject] private ItemDataManager _itemDataManager;
        

        public GraphicRaycaster UIShopRayCaster => _uiShopRaycaster;
        public EventSystem EventSystem => _eventSystem;

        public Transform ItemCoordinate => _itemCoordinate;
        protected override void AwakeInit()
        {
            base.AwakeInit();
            Bind<TMP_Text>(typeof(ItemShopText));
            Bind<Image>(typeof(IconImages));
            _equipItemTapText = GetText((int)ItemShopText.EquipItemTapText);
            _consumableItemTapText = GetText((int)ItemShopText.ConsumableItemTapText);
            _etcItemTapText = GetText((int)ItemShopText.ETCItemTapText);
            _playerHasGoldText = GetText((int)ItemShopText.PlayerHasGoldText);
            
            _equipItemIcon = GetImage((int)IconImages.EquipItemTap);
            _consumableItemIcon = GetImage((int)IconImages.ConsumableItemTap);
            _etcItemIcon = GetImage((int)(IconImages.ETCItemTap));
            _tabFocusLine = GetImage((int)(IconImages.TabFocusLine));

            _popupUIManager.AddImportant_Popup_UI(this);
            //클릭한 아이템 탭 오브젝트안에 있는 텍스트를 불러오기 위한 딕셔너리,
            //클릭할때마다 GetComponentinChildren를 호출하기 싫어서 만듦
            _findGameObjectToTMPTextDict = new Dictionary<GameObject, (TMP_Text, ItemType)>()
            {
                {_equipItemTapText.transform.parent.gameObject,(_equipItemTapText,ItemType.Equipment)},
                {_consumableItemTapText.transform.parent.gameObject,(_consumableItemTapText,ItemType.Consumable)},
                {_etcItemTapText.transform.parent.gameObject,(_etcItemTapText,ItemType.ETC)},
            };

            _focusColor = _focusTabColorHexcode.HexCodetoConvertColor();
            _nonFocusColor = _nonfocusTabColorHexcode.HexCodetoConvertColor();
            _currentFocusText = _equipItemTapText;
            _itemCoordinate = gameObject.FindChild<ItemShopContentCoordinate>(null, true).transform;

            _uiShopRaycaster = GetComponent<GraphicRaycaster>();
            _eventSystem = FindAnyObjectByType<EventSystem>();


            if (Managers.GameManagerEx.Player == null || Managers.GameManagerEx.Player.GetComponent<PlayerStats>() == null)
            {
                Managers.GameManagerEx.OnPlayerSpawnEvent += InitializePlayerStatEvent;
            }
            else
            {
              _playerStats = Managers.GameManagerEx.Player.GetComponent<PlayerStats>();
            }
        }

        public void InitializePlayerStatEvent(PlayerStats playerstats)
        {
            _playerStats = playerstats;
            OnEnableInit();
        }

        protected override void OnEnableInit()
        {
            base.OnEnableInit();
            if (_playerStats == null)
                return;
            _closePopupUI.performed += CloseDecriptionWindow;
            _playerStats.PlayerHasGoldChangeEvent += UpdateHasGoldChanged;
            BindEvent(_equipItemIcon.gameObject, ClickToTab);
            BindEvent(_consumableItemIcon.gameObject, ClickToTab);
            BindEvent(_etcItemIcon.gameObject, ClickToTab);
            UpdateHasGoldChanged(_playerStats.Gold);
        }

        protected override void OnDisableInit()
        {
            if (_playerStats == null)
                return;
            base.OnDisableInit();
            _closePopupUI.performed -= CloseDecriptionWindow;
            _playerStats.PlayerHasGoldChangeEvent -= UpdateHasGoldChanged;
            UnBindEvent(_equipItemIcon.gameObject, ClickToTab);
            UnBindEvent(_consumableItemIcon.gameObject, ClickToTab);
            UnBindEvent(_etcItemIcon.gameObject, ClickToTab);
            CloseDecriptionWindow();
        }
        public void UpdateHasGoldChanged(int gold)
        {
            _playerHasGoldText.text = gold.ToString();
        }
        public void CloseDecriptionWindow(InputAction.CallbackContext context)
        {
            CloseDecriptionWindow();
        }
        public void CloseDecriptionWindow()
        {
            if (_sceneUIManager.Try_Get_Scene_UI(out UIDescription description))
            {
                description.UI_DescriptionDisable();
                description.SetdecriptionOriginPos();
            }
        }

        public void ClickToTab(PointerEventData eventData)
        {
            _currentFocusText.color = _nonFocusColor;

            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
                _moveCoroutine = null;
            }
            _moveCoroutine = StartCoroutine(MovetoFocusTab(_tabFocusLine.transform, eventData.pointerPress.transform));
            if (_findGameObjectToTMPTextDict.TryGetValue(eventData.pointerPress, out (TMP_Text, ItemType) value))
            {
                (TMP_Text focusText, ItemType type) = value;
                focusText.color = _focusColor;
                _currentFocusText = focusText;

                ShowItemTypeForSelectedTab(type);
            }
        }

        public void ShowItemTypeForSelectedTab(ItemType type)
        {
            foreach (Transform childTr in _itemCoordinate)
            {
                if (childTr.gameObject.GetComponent<UIShopItemComponent>().ItemType == type)
                    childTr.gameObject.SetActive(true);
                else
                    childTr.gameObject.SetActive(false);
            }
        }

        IEnumerator MovetoFocusTab(Transform originTr, Transform tarGetTr)
        {
            float elapsedTime = 0f;
            float duration = 0.4f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float ratio = Mathf.Clamp01(elapsedTime / duration); // 0 ~ 1 비율 계산
                _tabFocusLine.transform.position = Vector3.Lerp(originTr.position, tarGetTr.position, ratio);
                yield return null;
            }
            elapsedTime = 0f;
        }

        protected override void StartInit()
        {
            if (Managers.GameManagerEx.Player == null)
            {
                Managers.GameManagerEx.OnPlayerSpawnEvent += (playerStats) =>
                {
                    Managers.GameManagerEx.Player.GetComponent<PlayerStats>();
                    UpdateHasGoldChanged(_playerStats.Gold);
                };
            }
            else
            {
                _playerStats = Managers.GameManagerEx.Player.GetComponent<PlayerStats>();
                UpdateHasGoldChanged(_playerStats.Gold);
            }
            RandomItemRespawn();
            ShowItemTypeForSelectedTab(ItemType.Equipment);
        }


        public void RandomItemRespawn()
        {
            for (int i = 0; i < 10; i++)
            {
                _itemDataManager.GetRandomItem(typeof(ItemConsumable)).MakeShopItemComponent(_popupUIManager,Random.Range(10, 20), null, Random.Range(1, 5));
                _itemDataManager.GetRandomItem(typeof(ItemEquipment)).MakeShopItemComponent(_popupUIManager,Random.Range(10, 20));
            }
        }
    }
}
