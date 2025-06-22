using GameManagers;
using Stats;
using Stats.BaseStats;
using TMPro;
using UnityEngine.UI;
using Zenject;

namespace UI.Scene.SceneUI
{
    public class UIPlayerInfo : UIScene
    {
        [Inject] GameManagerEx _gameManagerEx;
        private Slider _hpSlider;
        private TMP_Text _hpText;
        private TMP_Text _levelText;
        private PlayerStats _playerStats;
        private TMP_Text _playerNameText;

        enum HpSlider
        {
            PlayerHpSlider,
        }
        enum UserText
        {
            HpText,
            LevelText,
            PlayerNameText
        }
        protected override void AwakeInit()
        {
            base.AwakeInit();
            Bind<Slider>(typeof(HpSlider));
            Bind<TMP_Text>(typeof(UserText));

            _hpSlider = Get<Slider>((int)(HpSlider.PlayerHpSlider));
            _hpText = GetText((int)UserText.HpText);
            _levelText = GetText((int)UserText.LevelText);
            _playerNameText = GetText((int)UserText.PlayerNameText);


        }
        protected override void StartInit()
        {

            if(_gameManagerEx.Player == null)
            {

                _gameManagerEx.OnPlayerSpawnEvent += InitalizePlayerInfo;
                _gameManagerEx.OnPlayerSpawnEvent += UpdateUI;
            }
            else
            {
                _playerStats = _gameManagerEx.Player.gameObject.GetComponent<PlayerStats>();
                InitalizePlayerInfo(_playerStats);
                UpdateUI(_playerStats);
            }
        }

        private void UpdateUI(PlayerStats stats)//이렇게 한 이유는 호스트는 CharacterBaseStat이 바로 넘어오는데 게스트는 못넘어옴 그래서 호스트는 이렇게 초기화 하고 게스트는 이벤트에서 처리
        {
            if (stats.CharacterBaseStats.Equals(default(CharacterBaseStat)))
                return;
            UpdateUIInfo(stats.CharacterBaseStats);
        }

        private void InitalizePlayerInfo(PlayerStats stats)
        {
            _playerStats = stats;
            stats.CurrentHpValueChangedEvent += UpdateCurrentHpValue;
            stats.MaxHpValueChangedEvent += UpdateCurrentMaxHpValue;
            stats.DoneBaseStatsLoading += UpdateUIInfo;
        }

        private void UpdateCurrentMaxHpValue(int preCurrentMaxHp, int currentMaxHp)
        {
            _hpSlider.value = (float)_playerStats.Hp / (float)currentMaxHp;
            _hpText.text = $"{_playerStats.Hp}/{currentMaxHp}";
        }

        private void UpdateCurrentHpValue(int preCurrentHp, int currentHp)
        {
            if (_playerStats.MaxHp == default)
                return;

            _hpSlider.value = (float)currentHp / (float)_playerStats.MaxHp;
            _hpText.text = $"{currentHp}/{_playerStats.MaxHp}";
        }



        public void UpdateUIInfo(CharacterBaseStat stat)
        {
            _hpText.text = $"{stat.Hp}/{stat.MaxHp}";
            _hpSlider.value = (float)stat.Hp / (float)stat.MaxHp;
            _levelText.text = _playerStats.Level.ToString();
            _playerNameText.text = _playerStats.Name.ToString();
        }
    }
}
