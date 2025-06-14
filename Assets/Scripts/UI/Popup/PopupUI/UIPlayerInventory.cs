using Data.Item;
using GameManagers;
using Stats;
using Stats.BaseStats;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Util;

namespace UI.Popup.PopupUI
{
    public class UIPlayerInventory : UIPopup, IPopupHandler
    {
        private PlayerStats _ownerPlayerStats;
        private TMP_Text _playerName;
        private TMP_Text _playerLevel;
        private TMP_Text _currentGold;
        private TMP_Text _hpStatText;
        private TMP_Text _attackStatText;
        private TMP_Text _defenseStatText;
        private GameObject _equipMent;
        private Transform _windowPanel;
        private Canvas _inventoryCanvas;
    
        private Vector3 _initialEquipPosition;
        private Vector2 _initialMousePosition;
        private Vector3 _initialWindowPosition;//인벤토리의 초기위치를 담는곳
        private Transform _itemInventoryTr;

        private GraphicRaycaster _uiInventoryRaycaster;
        private EventSystem _eventSystem;
        public Transform ItemInventoryTr => _itemInventoryTr;
        public GraphicRaycaster UIInventoryRayCaster=> _uiInventoryRaycaster;
        public EventSystem EventSystem => _eventSystem;
        public PlayerStats OwnerPlayerStats
        {
            get
            {
                if(_ownerPlayerStats == null )
                {
                    if (Managers.GameManagerEx.Player != null && Managers.GameManagerEx.Player.TryGetComponent(out PlayerStats stats) == true)
                    {
                        _ownerPlayerStats = stats;
                    }
                }
                return _ownerPlayerStats;
            }
        }

        public bool IsVisible => _inventoryCanvas.enabled;

        enum EquipmentGo
        {
            Equipment
        }

        enum PanelTr
        {
            EquipSlotR,
            EquipSlotL,
            Player,
            LeftPanelBottom,
            RightPanelBottom,
            WindowPanel
        }

        protected override void AwakeInit()
        {
            base.AwakeInit();
            Managers.UIManager.AddImportant_Popup_UI(this);
            Bind<Transform>(typeof(PanelTr));
            Bind<GameObject>(typeof(EquipmentGo));
            Transform playerTr = Get<Transform>((int)PanelTr.Player);
            GameObject playerInfoTr = Utill.FindChild(playerTr.gameObject, "Player_Info_Panel");

            _playerName = Utill.FindChild(playerInfoTr, "PlayerName").GetComponent<TMP_Text>();
            //이름 초기화
            _playerLevel = Utill.FindChild(playerInfoTr, "PlayerLevelText").GetComponent<TMP_Text>();
            //레벨 초기화
            Transform leftPanelBottom = Get<Transform>((int)PanelTr.LeftPanelBottom);
            _currentGold = Utill.FindChild(leftPanelBottom.gameObject, "Coin_Text", true).GetComponent<TMP_Text>();
            //골드 초기화
            Transform rightPanelBottom = Get<Transform>((int)PanelTr.RightPanelBottom);
            _hpStatText = Utill.FindChild(rightPanelBottom.gameObject, "HP_Stat_Text", true).GetComponent<TMP_Text>();
            _attackStatText = Utill.FindChild(rightPanelBottom.gameObject, "Attack_Stat_Text", true).GetComponent<TMP_Text>();
            _defenseStatText = Utill.FindChild(rightPanelBottom.gameObject, "Defense_Stat_Text", true).GetComponent<TMP_Text>();
            //스탯 초기화

            _windowPanel = Get<Transform>((int)PanelTr.WindowPanel);
            _equipMent = Get<GameObject>((int)EquipmentGo.Equipment);

            _initialWindowPosition = (_equipMent.transform as RectTransform).localPosition;

            _itemInventoryTr = Utill.FindChild<InventoryContentCoordinate>(gameObject, null, true).transform;
            _inventoryCanvas = GetComponent<Canvas>();

            _uiInventoryRaycaster = GetComponent<GraphicRaycaster>();
            _eventSystem = FindAnyObjectByType<EventSystem>();

        }
        protected override void StartInit()
        {
            RectTransform parentRectTransform = transform as RectTransform;

            // 드래그 시작 시 초기 위치 저장
            _windowPanel.gameObject.AddUIEvent((PointerEventData eventData) =>
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRectTransform,
                    eventData.position,
                    null,
                    out _initialMousePosition
                );
                _initialEquipPosition = _equipMent.transform.localPosition;
            }, Define.UIEvent.DragBegin);

            // 드래그 중 창 위치 업데이트
            _windowPanel.gameObject.AddUIEvent((PointerEventData eventData) =>
            {
                Vector2 currentMousePosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRectTransform,
                    eventData.position,
                    null,
                    out currentMousePosition
                );

                Vector2 offset = currentMousePosition - _initialMousePosition;
                _equipMent.transform.localPosition = _initialEquipPosition + (Vector3)offset;
            }, Define.UIEvent.Drag);
            UpdateStats();
            UpdateGoldUI(OwnerPlayerStats.Gold);

            ClosePopup();
            //아이템을 로드하기 위해 게임오브젝트는 켜두는데 캔버스만 꺼둠.

        }

        private void UpdatePlayerLevelAndNickName(CharacterBaseStat stat)
        {
            _playerName.text = OwnerPlayerStats.Name;
            _playerLevel.text = $"LV : {OwnerPlayerStats.Level}";
        }

        public void CloseDecriptionWindow(InputAction.CallbackContext context)
        {
            CloseDecriptionWindow();
        }
        public void CloseDecriptionWindow()
        {
            if (Managers.UIManager.Try_Get_Scene_UI(out UI_Description description))
            {
                description.UI_DescriptionDisable();
                description.SetdecriptionOriginPos();
            }
        }

        protected override void OnEnableInit()
        {
            base.OnEnableInit();
            _closePopupUI.performed += CloseDecriptionWindow;
            if(OwnerPlayerStats != null)
            {
                SubscribePlayerEvent();
                UpdateGoldUI(OwnerPlayerStats.Gold);
                UpdateStats();
                UpdatePlayerLevelAndNickName(OwnerPlayerStats.CharacterBaseStats);
            }
            //Managers.LootItemManager.LoadItemsFromLootStorage(_itemInventoryTr);
            _equipMent.transform.localPosition = _initialWindowPosition;

        }
        protected override void OnDisableInit()
        {
            base.OnDisableInit();
            _closePopupUI.performed -= CloseDecriptionWindow;
            if (OwnerPlayerStats != null)
            {
                DeSubscribePlayerEvent();
            }
            CloseDecriptionWindow();
        }


        private void SubscribePlayerEvent()
        {

            OwnerPlayerStats.CurrentHpValueChangedEvent += UpdateCurrentHpValue;
            OwnerPlayerStats.MaxHpValueChangedEvent += UpdateMaxHpValue;
            OwnerPlayerStats.AttackValueChangedEvent += UpdateAttackValue;
            OwnerPlayerStats.DefenceValueChangedEvent += UpdatedefenceValue;
            OwnerPlayerStats.PlayerHasGoldChangeEvent += UpdateGoldUI;
            OwnerPlayerStats.DoneBaseStatsLoading += UpdatePlayerLevelAndNickName;

        }
        private void DeSubscribePlayerEvent()
        {
            OwnerPlayerStats.CurrentHpValueChangedEvent -= UpdateCurrentHpValue;
            OwnerPlayerStats.MaxHpValueChangedEvent -= UpdateMaxHpValue;
            OwnerPlayerStats.AttackValueChangedEvent -= UpdateAttackValue;
            OwnerPlayerStats.DefenceValueChangedEvent -= UpdatedefenceValue;
            OwnerPlayerStats.PlayerHasGoldChangeEvent -= UpdateGoldUI;
            OwnerPlayerStats.DoneBaseStatsLoading -= UpdatePlayerLevelAndNickName;

        }
        public void UpdateStats()
        {
            _hpStatText.text = $"{OwnerPlayerStats.Hp} / {OwnerPlayerStats.MaxHp}";
            _attackStatText.text = OwnerPlayerStats.Attack.ToString();
            _defenseStatText.text = OwnerPlayerStats.Defence.ToString();
        }

        private void UpdateGoldUI(int hasgold)
        {
            _currentGold.text = hasgold.ToString();
        }

        private void UpdateCurrentHpValue(int preCurrentHpValue,int currentHp)
        {
            _hpStatText.text = $"{currentHp} / {OwnerPlayerStats.MaxHp}";
        }
        private void UpdateMaxHpValue(int preMaxHpValue ,int maxHp)
        {
            _hpStatText.text = $"{OwnerPlayerStats.Hp} / {maxHp}";
        }
        private void UpdateAttackValue(int preAttackValue, int attack)
        {
            _attackStatText.text = attack.ToString();
        }
        private void UpdatedefenceValue(int preDefenceValue, int defence)
        {
            _defenseStatText.text = defence.ToString();
        }

        public void ShowPopup()
        {
            _inventoryCanvas.enabled = true;
        }

        public void ClosePopup()
        {
            _inventoryCanvas.enabled = false;
        }
    }
}
