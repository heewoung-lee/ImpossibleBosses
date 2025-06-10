using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Shop : UI_Popup
{
    private readonly string FOCUS_TAB_COLOR_HEXCODE = "#F6E19C";
    private readonly string NONFOCUS_TAB_COLOR_HEXCODE = "#BEB5B6";
    enum ItemShopText
    {
        EquipItem_TapText,
        ConsumableItem_TapText,
        ETCItem_TapText,
        PlayerHasGoldText
    }

    enum IconImages
    {
        EquipItem_Tap,
        ConsumableItem_Tap,
        ETCItem_Tap,
        TabFocusLine
    }


    Dictionary<GameObject, (TMP_Text, ItemType)> _findGameObjectToTMP_TextDict;

    private TMP_Text _equipItem_TapText;
    private TMP_Text _consumableItem_TapText;
    private TMP_Text _etcItem_TapText;
    private TMP_Text _playerHasGoldText;
    private TMP_Text _currentFocusText;

    private Image _equipItem_icon;
    private Image _consumableItem_icon;
    private Image _etcItem_icon;
    private Image _tabFocusLine;

    private Transform _itemCoordinate;
    private PlayerStats _playerStats;
    private Coroutine _moveCoroutine;

    private Color _focus_Color;
    private Color _nonFocus_Color;

    private GraphicRaycaster _ui_Shop_Raycaster;
    private EventSystem _eventSystem;

    public GraphicRaycaster UI_Shop_RayCaster => _ui_Shop_Raycaster;
    public EventSystem EventSystem => _eventSystem;

    public Transform ItemCoordinate => _itemCoordinate;
    protected override void AwakeInit()
    {
        base.AwakeInit();
        Bind<TMP_Text>(typeof(ItemShopText));
        Bind<Image>(typeof(IconImages));
        _equipItem_TapText = GetText((int)ItemShopText.EquipItem_TapText);
        _consumableItem_TapText = GetText((int)ItemShopText.ConsumableItem_TapText);
        _etcItem_TapText = GetText((int)ItemShopText.ETCItem_TapText);
        _playerHasGoldText = GetText((int)ItemShopText.PlayerHasGoldText);

        _equipItem_icon = GetImage((int)IconImages.EquipItem_Tap);
        _consumableItem_icon = GetImage((int)IconImages.ConsumableItem_Tap);
        _etcItem_icon = GetImage((int)(IconImages.ETCItem_Tap));
        _tabFocusLine = GetImage((int)(IconImages.TabFocusLine));

        Managers.UI_Manager.AddImportant_Popup_UI(this);
        //클릭한 아이템 탭 오브젝트안에 있는 텍스트를 불러오기 위한 딕셔너리,
        //클릭할때마다 GetComponentinChildren를 호출하기 싫어서 만듦
        _findGameObjectToTMP_TextDict = new Dictionary<GameObject, (TMP_Text, ItemType)>()
        {
            {_equipItem_TapText.transform.parent.gameObject,(_equipItem_TapText,ItemType.Equipment)},
            {_consumableItem_TapText.transform.parent.gameObject,(_consumableItem_TapText,ItemType.Consumable)},
            {_etcItem_TapText.transform.parent.gameObject,(_etcItem_TapText,ItemType.ETC)},
        };

        _focus_Color = FOCUS_TAB_COLOR_HEXCODE.HexCodetoConvertColor();
        _nonFocus_Color = NONFOCUS_TAB_COLOR_HEXCODE.HexCodetoConvertColor();
        _currentFocusText = _equipItem_TapText;
        _itemCoordinate = gameObject.FindChild<ItemShopContentCoordinate>(null, true).transform;

        _ui_Shop_Raycaster = GetComponent<GraphicRaycaster>();
        _eventSystem = FindAnyObjectByType<EventSystem>();

        BindEvent(_equipItem_icon.gameObject, ClickToTab);
        BindEvent(_consumableItem_icon.gameObject, ClickToTab);
        BindEvent(_etcItem_icon.gameObject, ClickToTab);

        if (_playerStats == null)
        {
            Managers.SocketEventManager.DonePlayerSpawnEvent += InitializePlayerStatEvent;
        }
        else
        {
            _playerStats = Managers.GameManagerEx.Player.GetComponent<PlayerStats>();
        }
    }

    public void InitializePlayerStatEvent(GameObject player)
    {
        _playerStats = player.GetComponent<PlayerStats>();
        OnEnableInit();
    }

    protected override void OnEnableInit()
    {
        base.OnEnableInit();
        if (_playerStats == null)
            return;
        _close_Popup_UI.performed += CloseDecriptionWindow;
        _playerStats.PlayerHasGoldChangeEvent += UpdateHasGoldChanged;
        UpdateHasGoldChanged(_playerStats.Gold);
    }

    protected override void OnDisableInit()
    {
        if (_playerStats == null)
            return;
        base.OnDisableInit();
        _close_Popup_UI.performed -= CloseDecriptionWindow;
        _playerStats.PlayerHasGoldChangeEvent -= UpdateHasGoldChanged;
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
        if (Managers.UI_Manager.Try_Get_Scene_UI(out UI_Description description))
        {
            description.UI_DescriptionDisable();
            description.SetdecriptionOriginPos();
        }
    }

    public void ClickToTab(PointerEventData eventData)
    {
        _currentFocusText.color = _nonFocus_Color;

        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
        _moveCoroutine = StartCoroutine(MovetoFocusTab(_tabFocusLine.transform, eventData.pointerPress.transform));
        if (_findGameObjectToTMP_TextDict.TryGetValue(eventData.pointerPress, out (TMP_Text, ItemType) value))
        {
            (TMP_Text focusText, ItemType type) = value;
            focusText.color = _focus_Color;
            _currentFocusText = focusText;

            ShowItemTypeForSelectedTab(type);
        }
    }

    public void ShowItemTypeForSelectedTab(ItemType type)
    {
        foreach (Transform childTr in _itemCoordinate)
        {
            if (childTr.gameObject.GetComponent<UI_ShopItemComponent>().Item_Type == type)
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
            Managers.ItemDataManager.GetRandomItem(typeof(ItemConsumable)).MakeShopItemComponent(Random.Range(10, 20), null, Random.Range(1, 5));
            Managers.ItemDataManager.GetRandomItem(typeof(ItemEquipment)).MakeShopItemComponent(Random.Range(10, 20));
        }
    }
}
