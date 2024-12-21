using System;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]
public abstract class BaseStats : MonoBehaviour, IDamageable
{
    private int _hp;
    private int _maxHp;
    private int _attack;
    private int _defence;
    private float _movespeed;
    private bool _isCheckDead = false;

    public Action<int> Event_Hp;
    public Action<int> Event_MaxHp;
    public Action<int> Event_Attack;
    public Action<int> Event_Defence;
    public Action<int> Event_Attacked;

    public Action<float> Event_MoveSpeed;
    public Action Event_StatsLoaded;
    public Action Event_StatsChanged;

    public int Hp{ 
        get => _hp;
        protected set{
            _hp = value;
            _hp = Mathf.Clamp(_hp,0,_maxHp);
            Event_Hp?.Invoke(_hp);
            Event_StatsChanged?.Invoke();
        }
    }
    public void Plus_Current_Hp_Abillity(int value)
    {
        Hp += value;
    }



    public int MaxHp{
        get => _maxHp;
        protected set
        {
            _maxHp = value;
            _maxHp = Mathf.Clamp(_maxHp, 0, int.MaxValue);
            Event_MaxHp?.Invoke(_maxHp);
            Event_StatsChanged?.Invoke();
        }
    }
    public void Plus_MaxHp_Abillity(int value)
    {
        MaxHp += value;
    }




    public int Attack{ 
        get => _attack;
        protected set
        {
            _attack = value;
            _attack = Mathf.Clamp(_attack , 0, int.MaxValue);
            Event_Attack?.Invoke(_attack);
            Event_StatsChanged?.Invoke();
        }
    }
    public void Plus_Attack_Ability(int value)
    {
        Attack += value;
    }



    public int Defence{
        get => _defence;
        protected set
        {
            _defence = value;
            _defence = Mathf.Clamp(_defence, 0, int.MaxValue);
            Event_Defence?.Invoke(_defence);
            Event_StatsChanged?.Invoke();
        }
    }
    public void Plus_Defence_Abillity(int value)
    {
        Defence += value;
    }



    public float MoveSpeed { 
        get => _movespeed;
        protected set
        {
            _movespeed = value;
            _movespeed = Mathf.Clamp(_movespeed, 0, int.MaxValue);
            Event_MoveSpeed?.Invoke(_movespeed);
            Event_StatsChanged?.Invoke();
        }
    }


    public void Plus_MoveSpeed_Abillity(float value)
    {
       MoveSpeed += value;
    }

    protected abstract void SetStats();
    protected abstract void StartInit();
    protected void UpdateStat()
    {
        SetStats();
        Event_StatsLoaded?.Invoke();
    }

    private void Awake()
    {
        AwakeInit();
    }

    private void Start()
    {
        StartInit();
        UpdateStat();
    }

    protected virtual void AwakeInit()
    {
        _hp = 1000;
        _maxHp = 1000;
        _attack = 10;
        _defence = 5;
        _movespeed = 5.0f;
    }


    public void OnAttacked(IAttackRange attacker, int? spacialDamage = null)
    {
        if (_isCheckDead) return;

        int damage = 0;
        attacker.Owner_Transform.TryGetComponent(out BaseStats attackerStats);
        if (spacialDamage == null)
            damage = attackerStats.Attack;
        else
            damage = spacialDamage.Value;


        damage = Mathf.Max(0, damage - Defence);
        Hp -= damage;
        Event_Attacked?.Invoke(damage);
        if (Hp <= 0)
        {
            Hp = 0;
            OnDead(attackerStats);
            _isCheckDead = true;
        }
    }

    public void TestDamaged(int damege)
    {
        Hp -= damege;
    }
    //TODO:TEST메서드 사용후 지울 것
    protected abstract void OnDead(BaseStats attacker);
}
