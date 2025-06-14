using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using GameManagers;
using Stats;
using Stats.BaseStats;
using TMPro;
using UI.Scene;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Test.TestScripts.UnitTest.PlaySceneMockUnitTest;
public class UI_Player_Info : UIScene
{
    private Slider _hpSlider;
    private TMP_Text _hpText;
    private TMP_Text _levelText;
    private PlayerStats _playerStats;
    private TMP_Text _playerName_Text;

    enum HP_Slider
    {
        Player_HP_Slider,
    }
    enum User_text
    {
        HP_Text,
        MAX_HP_Text,
        Level_Text,
        PlayerName_Text
    }
    protected override void AwakeInit()
    {
        base.AwakeInit();
        Bind<Slider>(typeof(HP_Slider));
        Bind<TMP_Text>(typeof(User_text));

        _hpSlider = Get<Slider>((int)(HP_Slider.Player_HP_Slider));
        _hpText = GetText((int)User_text.HP_Text);
        _levelText = GetText((int)User_text.Level_Text);
        _playerName_Text = GetText((int)User_text.PlayerName_Text);


    }
    protected override void StartInit()
    {

        if(Managers.GameManagerEx.Player == null)
        {

            Managers.GameManagerEx.OnPlayerSpawnEvent += InitalizePlayerInfo;
            Managers.GameManagerEx.OnPlayerSpawnEvent += UpdateUI;
        }
        else
        {
            _playerStats = Managers.GameManagerEx.Player.gameObject.GetComponent<PlayerStats>();
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
        stats.CurrentHpValueChangedEvent += UpdateCurrentHPValue;
        stats.MaxHpValueChangedEvent += UpdateCurrentMaxHpValue;
        stats.DoneBaseStatsLoading += UpdateUIInfo;
    }

    private void UpdateCurrentMaxHpValue(int preCurrentMaxHp, int currentMaxHP)
    {
        _hpSlider.value = (float)_playerStats.Hp / (float)currentMaxHP;
        _hpText.text = $"{_playerStats.Hp}/{currentMaxHP}";
    }

    private void UpdateCurrentHPValue(int preCurrentHp, int currentHP)
    {
        if (_playerStats.MaxHp == default)
            return;

        _hpSlider.value = (float)currentHP / (float)_playerStats.MaxHp;
        _hpText.text = $"{currentHP}/{_playerStats.MaxHp}";
    }



    public void UpdateUIInfo(CharacterBaseStat stat)
    {
        _hpText.text = $"{stat.Hp}/{stat.MaxHp}";
        _hpSlider.value = (float)stat.Hp / (float)stat.MaxHp;
        _levelText.text = _playerStats.Level.ToString();
        _playerName_Text.text = _playerStats.Name.ToString();
    }
}
