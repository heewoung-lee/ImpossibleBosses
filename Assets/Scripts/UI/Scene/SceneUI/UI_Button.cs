using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button : UI_Scene
{
    private Button _scoreButton;
    private TMP_Text _scoreText;
    private Image _scoreImage;

    private PlayerStats _playerStats;

    public ItemGeneratingType itemGeneratingType = ItemGeneratingType.All;

    enum Buttons
    {
        ScoreButton
    }
    enum Texts
    {
        ScoreText
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
        _playerStats = Managers.GameManagerEx.Player.GetComponent<PlayerStats>();
        _scoreButton.onClick.AddListener(() =>
        {
            TestIteminInventort();
            TestPlayerDamaged();
        });

        BindEvent(_scoreImage.gameObject, (PointerEventData) =>
        {
            _scoreImage.gameObject.transform.position = PointerEventData.position;
        }, Define.UI_Event.Drag);
    }


    public void TestPlayerDamaged()
    {
        _playerStats.Exp += 5;
        Debug.Log(_playerStats.Exp + "플레이어 경험치");
        Debug.Log($"플레이어 레벨{_playerStats.Level}");

        _playerStats.Gold += 5;
        _playerStats.OnAttacked(_playerStats,80);
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

}
