using System;
using System.Collections.Generic;
using UnityEngine;
public class PlayerStats : BaseStats, IAttackRange
{
    private Dictionary<int, PlayerStat> _statDict;
    private int _level;
    private int _currentexp;
    private int _gold;
    private float _viewAngle;
    private float _viewDistance;

   private string _playerName;

    public Action PlayerDeadEvent;
    public Action<int> PlayerHasGoldChangeEvent;
    private LayerMask _targetLayer;


    public string Name
    {
        get
        {
            if (_playerName == null)
            {
                _playerName = Managers.LobbyManager.CurrentPlayerInfo.PlayerNickName;
            }
            return _playerName;
        }
    }
    public int Gold
    {
        get => _gold;
        set
        {
            _gold = value;
            _gold = Mathf.Clamp(_gold, 0,int.MaxValue);
            PlayerHasGoldChangeEvent?.Invoke(_gold);
        }
    }
    public float ViewAngle { get => _viewAngle; }
    public float ViewDistance { get => _viewDistance; }

    public int Level { get => _level; }
    public Transform Owner_Transform => transform;
    public LayerMask TarGetLayer => _targetLayer;
    public Vector3 AttackPosition => transform.position;
    protected override void AwakeInit()
    {
        base.AwakeInit();
        _level = 1;
        _currentexp = 0;
        _gold = 0;
    }
    protected override void StartInit()
    {
        _statDict = Managers.DataManager.AllDataDict[typeof(PlayerStat)] as Dictionary<int, PlayerStat>;
        _targetLayer = LayerMask.GetMask("Monster");

        if (IsOwner == false)
            return;

        SetStats();
    }
    public int Exp
    {
        get => _currentexp;
        set
        {
            _currentexp = value;
            //레벨업 체크
            while (true)
            {
                PlayerStat stat;
                if (_statDict.TryGetValue(_level + 1, out stat) == false)
                    break;//만렙이면 멈추기

                if (_currentexp < stat.xpRequired)
                    break;

                else if (_currentexp >= stat.xpRequired) // 100/20
                {
                    _currentexp -= stat.xpRequired;
                    _level++;
                    UpdateStat();
                    Managers.VFX_Manager.GenerateParticle("Prefabs/Player/SkillVFX/Level_up", gameObject.transform);
                }
            }
        }
    }


    protected override void OnDead(BaseStats attacker)
    {
        if (IsOwner)
        {
            PlayerDeadEvent.Invoke();
        }
    }

    protected override void SetStats()
    {
        PlayerStat stat = _statDict[_level];
        CharacterBaseStat baseStat = new CharacterBaseStat(stat.hp, stat.hp, stat.attack, stat.defence, stat.speed);
        CharacterBaseStats = baseStat;
         _viewAngle = stat.viewAngle;
        _viewDistance = stat.viewDistance;
    }
}
