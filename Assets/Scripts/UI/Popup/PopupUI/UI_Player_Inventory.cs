using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Player_Inventory : UI_Popup
{
    private PlayerStats _stat;
    private TMP_Text _playerName;
    private TMP_Text _playerLevel;
    private TMP_Text _currentGold;
    private TMP_Text _hp_Stat_Text;
    private TMP_Text _attack_Stat_Text;
    private TMP_Text _defense_Stat_Text;
    private Button _windowClose_Button;
    private GameObject _equipMent;
    private Transform _windowPanel;
    private Canvas _inventoryCanvas;

    private Vector3 _initialEquipPosition;
    private Vector2 _initialMousePosition;
    private Vector3 _initialWindowPosition;//인벤토리의 초기위치를 담는곳
    private Transform _itemInventoryTr;
    private Transform _lootitemStorage;

    private GraphicRaycaster _ui_inventory_Raycaster;
    private EventSystem _eventSystem;

    public Transform ItemInventoryTr => _itemInventoryTr;
    public GraphicRaycaster UI_Inventory_RayCaster=> _ui_inventory_Raycaster;
    public EventSystem EventSystem => _eventSystem;
    public Transform InventoryOnwer => _stat.transform;

    
    enum Equipment_Go
    {
        Equipment
    }

    enum Panel_Tr
    {
        EquipSlot_R,
        EquipSlot_L,
        Player,
        Left_Panel_Bottom,
        Right_Panel_TapMenu,
        Inventory_ScrollRect,
        Right_Panel_Bottom,
        Window_Panel
    }

    protected override void AwakeInit()
    {
        base.AwakeInit();
        Managers.UI_Manager.AddImportant_Popup_UI(this);
        Bind<Transform>(typeof(Panel_Tr));
        Bind<GameObject>(typeof(Equipment_Go));
        Transform playerTr = Get<Transform>((int)Panel_Tr.Player);
        GameObject playerInfoTr = Utill.FindChild(playerTr.gameObject, "Player_Info_Panel");

        _playerName = Utill.FindChild(playerInfoTr, "PlayerName").GetComponent<TMP_Text>();
        //이름 초기화
        _playerLevel = Utill.FindChild(playerInfoTr, "PlayerLevelText").GetComponent<TMP_Text>();
        //레벨 초기화
        Transform left_Panel_Bottom = Get<Transform>((int)Panel_Tr.Left_Panel_Bottom);
        _currentGold = Utill.FindChild(left_Panel_Bottom.gameObject, "Coin_Text", true).GetComponent<TMP_Text>();
        //골드 초기화
        Transform right_Panel_Bottom = Get<Transform>((int)Panel_Tr.Right_Panel_Bottom);
        _hp_Stat_Text = Utill.FindChild(right_Panel_Bottom.gameObject, "HP_Stat_Text", true).GetComponent<TMP_Text>();
        _attack_Stat_Text = Utill.FindChild(right_Panel_Bottom.gameObject, "Attack_Stat_Text", true).GetComponent<TMP_Text>();
        _defense_Stat_Text = Utill.FindChild(right_Panel_Bottom.gameObject, "Defense_Stat_Text", true).GetComponent<TMP_Text>();
        //스탯 초기화

        _windowPanel = Get<Transform>((int)Panel_Tr.Window_Panel);
        _equipMent = Get<GameObject>((int)Equipment_Go.Equipment);

        _initialWindowPosition = (_equipMent.transform as RectTransform).localPosition;

        _itemInventoryTr = Utill.FindChild<InventoryContentCoordinate>(gameObject,null,true).transform;
        _inventoryCanvas = GetComponent<Canvas>();

        _ui_inventory_Raycaster = GetComponent<GraphicRaycaster>();
        _eventSystem = FindAnyObjectByType<EventSystem>();

        _lootitemStorage = Managers.LootItemManager.LootingItemRoot;
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
        }, Define.UI_Event.DragBegin);

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
        }, Define.UI_Event.Drag);
        _stat = Managers.GameManagerEx.Player.GetComponent<PlayerStats>();
        UpdateStats();
        _stat.Event_StatsChanged += UpdateStats;
    }
    public void CloseDecriptionWindow(InputAction.CallbackContext context)
    {
        CloseDecriptionWindow();
    }

    public void CloseDecriptionWindow()
    {
        UI_Description description = null;
        if (description = Managers.UI_Manager.Get_Scene_UI<UI_Description>())
        {
            description.gameObject.SetActive(false);
            description.SetdecriptionOriginPos();
        }
    }

    protected override void OnEnableInit()
    {
        base.OnEnableInit();
        _close_Popup_UI.performed += CloseDecriptionWindow;
        if(_stat != null)
        {
            _stat.Event_StatsChanged += UpdateStats;
            UpdateStats();
        }
        LoadItemsFromLootStorage();
        _equipMent.transform.localPosition = _initialWindowPosition;
    }
    protected override void OnDisableInit()
    {
        base.OnDisableInit();
        _close_Popup_UI.performed -= CloseDecriptionWindow;
        if (_stat != null)
        {
            _stat.Event_StatsChanged -= UpdateStats;
        }
        CloseDecriptionWindow();
    }

    private void LoadItemsFromLootStorage()
    {
        if (_lootitemStorage.childCount <= 0)
            return;


        for (int i = _lootitemStorage.childCount - 1; i >= 0; i--)
        {
            Transform child = _lootitemStorage.GetChild(i);
            UI_ItemComponent_Inventory lootItem = child.GetComponent<UI_ItemComponent_Inventory>();
            if (lootItem != null)
            {
                lootItem.transform.SetParent(_itemInventoryTr);

                if (lootItem is UI_ItemComponent_Consumable)
                {
                    (lootItem as UI_ItemComponent_Consumable).CombineConsumableItems();
                }
            }
        }
    }


    public void UpdateStats()
    {
        _playerName.text = _stat.Name;
        _playerLevel.text = $"LV : {_stat.Level}";
        _currentGold.text = _stat.Gold.ToString();
        _hp_Stat_Text.text = $"{_stat.Hp} / {_stat.MaxHp}";
        _attack_Stat_Text.text = _stat.Attack.ToString();
        _defense_Stat_Text.text = _stat.Defence.ToString();
    }

}
