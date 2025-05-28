using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CREATE_ITEM_AND_GOLD_Button : UI_Scene
{
    private Button _scoreButton;
    private Button _moveSceneButton;
    private TMP_Text _scoreText;

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
        ScoreButton,
        MoveDownTownScene
    }
    enum Texts
    {
        ScoreText,
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

        _scoreButton = GetButton((int)Buttons.ScoreButton);
        _moveSceneButton = GetButton((int)Buttons.MoveDownTownScene);
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

        _scoreButton.onClick.AddListener(TestButtonClick);
        _moveSceneButton.onClick.AddListener(MoveScene);

        void TestButtonClick()
        {
            TestIteminInventort();
            TestGetGold();
            TestGetExp();
            TestGetDamaged();
            //await Managers.LobbyManager.ShowUpdatedLobbyPlayers();
            //_ = FindMyJoinCodeAsync();
            Debug.Log(Managers.GameManagerEx.Player.transform.position+"플레이어 좌표");
        }
        void MoveScene()
        {
            (Managers.SceneManagerEx.GetCurrentScene as ISceneController).SceneMoverController.ISceneBehaviour.nextscene.MoveScene();
        }
    }
    
    public void TestGetGold() => PlayerStats.Gold += 5;
    public void TestGetDamaged() => PlayerStats.OnAttacked(_playerStats,2);
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
