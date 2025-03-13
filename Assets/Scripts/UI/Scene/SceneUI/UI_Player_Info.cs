using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UI_Player_Info : UI_Scene
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
        _playerStats = Managers.GameManagerEx.Player.gameObject.GetComponent<PlayerStats>();
        InitalizePlayerInfo();
        UpdateUIInfo(_playerStats.CharacterBaseStats);
    }

    private void InitalizePlayerInfo()
    {
        _playerStats.CurrentHPValueChangedEvent += UpdateCurrentHPValue;

        _playerStats.Done_Base_Stats_Loading -= UpdateUIInfo;
        _playerStats.Done_Base_Stats_Loading += UpdateUIInfo;
    }

    private void UpdateCurrentHPValue(int currentHP)
    {
        _hpSlider.value = (float)currentHP / (float)_playerStats.MaxHp;
        _hpText.text = $"{currentHP}/{_playerStats.MaxHp}";
    }

    public void UpdateUIInfo(CharacterBaseStat stat)
    {
        _hpText.text = $"{stat.hp}/{stat.maxHp}";
        _hpSlider.value = stat.hp / stat.maxHp;
        _levelText.text = _playerStats.Level.ToString();
        _playerName_Text.text = _playerStats.Name.ToString();
    }
}
