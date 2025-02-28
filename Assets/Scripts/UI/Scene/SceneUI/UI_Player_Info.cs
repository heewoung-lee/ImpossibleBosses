using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[RequireComponent(typeof(PlayerStats))]
public class UI_Player_Info : UI_Scene
{
    private Slider _hpSlider;
    private TMP_Text _hpText;
    private TMP_Text _levelText;
    private PlayerStats _stats;
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
        Managers.SocketEventManager.PlayerSpawnInitalize += InitializePlayerInfo;
    }

    public void InitializePlayerInfo(GameObject player)
    {
        _stats = player.GetComponent<PlayerStats>();
        _stats.Event_StatsChanged -= SetHpUI;
        _stats.Event_StatsChanged += SetHpUI;

        _stats.Event_StatsLoaded -= UpdateUIInfo;
        _stats.Event_StatsLoaded += UpdateUIInfo;

        _stats.Event_StatsLoaded.Invoke();
    }

    public void SetHpUI()
    {
        _hpSlider.value = (float)_stats.Hp / (float)_stats.MaxHp;
        _hpText.text = $"{_stats.Hp}/{_stats.MaxHp}";
    }

    public void UpdateUIInfo()
    {
        _hpText.text = $"{_stats.Hp}/{_stats.MaxHp}";
        _hpSlider.value = (float)_stats.Hp / (float)_stats.MaxHp;
        _levelText.text = _stats.Level.ToString();
        _playerName_Text.text = _stats.Name.ToString();
    }
}
