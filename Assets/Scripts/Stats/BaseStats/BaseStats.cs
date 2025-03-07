using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
[DisallowMultipleComponent]
public abstract class BaseStats : NetworkBehaviour, IDamageable
{
    private int _maxHp;
    private int _attack;
    private int _defence;
    private float _movespeed;
    private bool _isCheckDead = false;

    public Action<int> Event_Attacked;
    public Action Event_StatsLoaded;
    public Action Event_StatsChanged;

    public NetworkVariable<int> playerHpValue = new NetworkVariable<int>
        (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
   

    public int Hp{ 
        get => playerHpValue.Value;
        protected set{
            if (IsSpawned == false)
                return;
            
            if (IsServer)
            {
                playerHpValue.Value = Mathf.Clamp(value, 0, _maxHp);
            }
            else
            {
                RequestHpChangedServerRpc(Mathf.Clamp(value, 0, _maxHp));
            }
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void RequestHpChangedServerRpc(int value)
    {
        playerHpValue.Value = value;
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
        _maxHp = 1000;
        _attack = 10;
        _defence = 5;
        _movespeed = 5.0f;
    }

    private void InitStatOption()
    {
        if (IsHost == false)
            return;

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerHpValue.OnValueChanged += HpValueChanged;
    }

  
    private void HpValueChanged(int previousValue, int newValue)
    {
        //int damage = previousValue - newValue;
        //if(damage>0)
        //Event_Attacked?.Invoke(damage);

        Event_StatsChanged?.Invoke();
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
    //TODO:TEST메서드 사용후 지울 것
    protected abstract void OnDead(BaseStats attacker);
}
