using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CREATE_ITEM_AND_GOLD_Button : UI_Scene
{
    private Button _scoreButton;
    private TMP_Text _scoreText;
    private Image _scoreImage;

    private PlayerStats _playerStats;


    public PlayerStats PlayerStats
    {
        get
        {
            if(_playerStats == null)
            {
                _playerStats = Managers.GameManagerEx.Player.GetComponent<PlayerStats>();
            }
            return _playerStats;
        }
    }

    public ItemGeneratingType itemGeneratingType = ItemGeneratingType.All;

    enum Buttons
    {
        ScoreButton
    }
    enum Texts
    {
        ScoreText,
    }
    enum Images
    {
        ScoreImage
    }


    public enum ItemGeneratingType
    {
        EquipMent,
        Consumable,
        All
    }
    protected override void AwakeInit()
    {
        base.AwakeInit();

        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        _scoreButton = GetButton((int)Buttons.ScoreButton);
        _scoreImage = GetImage((int)Images.ScoreImage);
        _scoreText = GetText((int)Texts.ScoreText);
    }
    protected override void StartInit()
    {
        InitalizeUI_Button();
    }

    public void IninitalizePlayerStats(GameObject player)
    {
        _playerStats = player.GetComponent<PlayerStats>();
    }
    public void InitalizeUI_Button()
    {
        void TestButtonClick()
        {
            TestIteminInventort();
            TestGetGold();
            TestGetExp();
            TestGetDamaged();
            //await Managers.LobbyManager.ShowUpdatedLobbyPlayers();
            _ = FindMyJoinCodeAsync();
        }

        _scoreButton.onClick.AddListener(TestButtonClick);
        BindEvent(_scoreImage.gameObject, (PointerEventData) =>
        {
            _scoreImage.gameObject.transform.position = PointerEventData.position;
        }, Define.UI_Event.Drag);
    }
    
    public void TestGetGold() => PlayerStats.Gold += 5;
    public void TestGetDamaged() => PlayerStats.OnAttacked(_playerStats, 5);
    public void TestGetExp() => PlayerStats.Exp += 5;

    public void TestGenerateBossSkill1()
    {
        GameObject stone = Managers.ResourceManager.Instantiate("Prefabs/Enemy/Boss/AttackPattren/BossSkill1");
        stone.transform.SetParent(Managers.VFX_Manager.VFX_Root_NGO, false);
        stone.transform.position = Managers.GameManagerEx.Player.transform.position + Vector3.up * 5f;
    }

    public void TestIteminInventort()
    {
        if (Managers.UI_Manager.GetImportant_Popup_UI<UI_Player_Inventory>().gameObject.activeSelf == false)
            return;

        switch (itemGeneratingType)
        {
            case ItemGeneratingType.EquipMent:
                IItem Equipmentitem = Managers.ItemDataManager.GetRandomItem(typeof(ItemEquipment)).MakeInventoryItemComponent();
                break;
            case ItemGeneratingType.Consumable:
                IItem consumableitem = Managers.ItemDataManager.GetRandomItem(typeof(ItemConsumable)).MakeInventoryItemComponent();
                break;
            case ItemGeneratingType.All:
                IItem item = Managers.ItemDataManager.GetRandomItemFromAll().MakeInventoryItemComponent();
                break;
        }
    }

    private async Task FindMyJoinCodeAsync()
    {
        Debug.Log($"내 조인코드는 {Managers.RelayManager.JoinCode}");
        Debug.Log($"로비의 조인코드는{(await Managers.LobbyManager.GetCurrentLobby()).Data["RelayCode"].Value}");
    }
}
