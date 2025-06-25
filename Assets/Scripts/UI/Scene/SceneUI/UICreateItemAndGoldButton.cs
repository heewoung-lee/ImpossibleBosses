using System.Threading.Tasks;
using Data.DataType.ItemType;
using Data.DataType.ItemType.Interface;
using GameManagers;
using GameManagers.Interface;
using GameManagers.Interface.GameManagerEx;
using GameManagers.Interface.Resources_Interface;
using GameManagers.Interface.UIManager;
using Stats;
using TMPro;
using UI.Popup.PopupUI;
using UnityEngine;
using UnityEngine.UI;
using Util;
using Zenject;

namespace UI.Scene.SceneUI
{
    public class UICreateItemAndGoldButton : UIScene
    {
        private Button _scoreButton;
        private Button _moveSceneButton;
        private TMP_Text _scoreText;

        private PlayerStats _playerStats;
        [Inject] private IUIPopupManager _uiPopupManager;
        [Inject] private IInstantiate _instantiate;
        [Inject] private ItemDataManager _itemDataManager;
        [Inject] IPlayerSpawnManager _gameManagerEx;
        
        public PlayerStats PlayerStats
        {
            get
            {
                if(_playerStats == null)
                {
                    _playerStats = _gameManagerEx.GetPlayer().GetComponent<PlayerStats>();
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
                Debug.Log(_gameManagerEx.GetPlayer().transform.position+"플레이어 좌표");
                //_gameManagerEx.GetPlayer().transform.position = Vector3.zero;
            }
            void MoveScene()
            {
                Managers.SceneManagerEx.GetCurrentScene.SceneSpawnBehaviour.Nextscene.MoveScene();
            }

            //void AllLevelup()
            //{
            //    int layerMask = LayerMask.GetMask("Player", "AnotherPlayer");
            //    Collider[] hitColliders = Physics.OverlapSphere(_gameManagerEx.GetPlayer().transform.position, 10f, layerMask);
            //    foreach (var collider in hitColliders)
            //    {
            //      Managers.VFX_Manager.GenerateParticle("Prefabs/Player/SkillVFX/Level_up", collider.transform);
            //    }
            //}
        }
    
        public void TestGetGold() => PlayerStats.Gold += 5;
        public void TestGetDamaged() => PlayerStats.OnAttacked(_playerStats,2);
        public void TestGetExp() => PlayerStats.Exp += 5;

        public void TestGenerateBossSkill1()
        {
            GameObject stone = _instantiate.InstantiateByPath("Prefabs/Enemy/Boss/AttackPattren/BossSkill1");
            stone.transform.SetParent(Managers.VFXManager.VFXRootNgo, false);
            stone.transform.position = _gameManagerEx.GetPlayer().transform.position + Vector3.up * 5f;
        }

        public void TestIteminInventort()
        {
            if (_uiPopupManager.GetImportant_Popup_UI<UIPlayerInventory>().gameObject.activeSelf == false)
                return;

            switch (itemGeneratingType)
            {
                case ItemGeneratingType.EquipMent:
                    IItem equipmentitem = _itemDataManager.GetRandomItem(typeof(ItemEquipment)).MakeInventoryItemComponent(_uiPopupManager);
                    break;
                case ItemGeneratingType.Consumable:
                    IItem consumableitem = _itemDataManager.GetRandomItem(typeof(ItemConsumable)).MakeInventoryItemComponent(_uiPopupManager);
                    break;
                case ItemGeneratingType.All:
                    IItem item = _itemDataManager.GetRandomItemFromAll().MakeInventoryItemComponent(_uiPopupManager);
                    break;
            }
        }

        private async Task FindMyJoinCodeAsync()
        {
            Debug.Log($"내 조인코드는 {Managers.RelayManager.JoinCode}");
            Debug.Log($"로비의 조인코드는{(await Managers.LobbyManager.GetCurrentLobby()).Data["RelayCode"].Value}");
        }
    }
}
