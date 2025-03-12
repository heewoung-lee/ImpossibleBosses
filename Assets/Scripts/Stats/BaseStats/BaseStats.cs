using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
[DisallowMultipleComponent]
public abstract class BaseStats : NetworkBehaviour, IDamageable
{
    private bool _isCheckDead = false;

    public Action<int> Event_Attacked;
    public Action Event_StatsLoaded;
    public Action Event_StatsChanged;
    public Action Done_Base_Stats_Loading;

    [SerializeField]
    private NetworkVariable<int> playerHpValue = new NetworkVariable<int>
        (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField]
    private NetworkVariable<int> playerMaxHpValue = new NetworkVariable<int>
        (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField]
    private NetworkVariable<int> playerAttackValue = new NetworkVariable<int>
        (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField]
    private NetworkVariable<int> playerDefenceValue = new NetworkVariable<int>
       (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField]
    private NetworkVariable<float> playerMoveSpeedValue = new NetworkVariable<float>
       (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public int Hp
    {
        get => playerHpValue.Value;
        protected set
        {
            PlayerHPValueChangedRpc(value);
        }
    }
    [Rpc(SendTo.Server)]
    public void PlayerHPValueChangedRpc(int value)
    {
        playerHpValue.Value = Mathf.Clamp(value, 0, MaxHp);
    }



    public int MaxHp
    {
        get => playerMaxHpValue.Value;
        protected set
        {
            PlayerMaxHPValueChangedRpc(value);
        }
    }
    [Rpc(SendTo.Server)]
    public void PlayerMaxHPValueChangedRpc(int value)
    {
        playerMaxHpValue.Value = Mathf.Clamp(value, 0, int.MaxValue);
    }



    public int Attack
    {
        get => playerAttackValue.Value;
        protected set
        {
            PlayerAttackValueChangedRpc(value);
        }
    }
    [Rpc(SendTo.Server)]
    public void PlayerAttackValueChangedRpc(int value)
    {
        playerAttackValue.Value = Mathf.Clamp(value, 0, int.MaxValue);
    }



    public int Defence
    {
        get => playerDefenceValue.Value;
        protected set
        {
            PlayerDefenceValueChangedRpc(value);
        }
    }
    [Rpc(SendTo.Server)]
    public void PlayerDefenceValueChangedRpc(int value)
    {
        playerDefenceValue.Value = Mathf.Clamp(value, 0, int.MaxValue);
    }



    public float MoveSpeed
    {
        get => playerMoveSpeedValue.Value;
        protected set
        {
            PlayeMoveSpeedValueChangedRpc(value);
        }
    }
    [Rpc(SendTo.Server)]
    public void PlayeMoveSpeedValueChangedRpc(float value)
    {
        playerMoveSpeedValue.Value = Mathf.Clamp(value, 0, float.MaxValue);
    }


    public void Plus_Current_Hp_Abillity(int value)
    {
        Hp += value;
    }
    public void Plus_Defence_Abillity(int value)
    {
        Defence += value;
    }
    public void Plus_Attack_Ability(int value)
    {
        Attack += value;
    }
    public void Plus_MaxHp_Abillity(int value)
    {
        MaxHp += value;
    }
    public void Plus_MoveSpeed_Abillity(float value)
    {
        MoveSpeed += value;
    }

    protected abstract void SetStats();
    protected abstract void StartInit();
    protected void UpdateStat()
    {
        if (IsOwner == false)
            return;


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
    }

    protected virtual void AwakeInit()
    {
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerHpValue.OnValueChanged += HpValueChanged;
        playerMaxHpValue.OnValueChanged += MaxHpValueChanged;
        playerAttackValue.OnValueChanged += AttackValueChanged;
        playerDefenceValue.OnValueChanged += DefenceValueChanged;
        playerMoveSpeedValue.OnValueChanged += MoveSpeedValueChanged;
        UpdateStat();
    }

    private void HpValueChanged(int previousValue, int newValue)
    {
        Event_StatsChanged?.Invoke();
        int damage = previousValue - newValue;
        if (damage > 0)
        {
            if(IsHost)
            OnAttackedClientRpc(damage);
        }
    }
    private void MaxHpValueChanged(int previousValue, int newValue)
    {
        Event_StatsChanged?.Invoke();
    }
    private void AttackValueChanged(int previousValue, int newValue)
    {
        Event_StatsChanged?.Invoke();
    }
    private void DefenceValueChanged(int previousValue, int newValue)
    {
        Event_StatsChanged?.Invoke();
    }
    private void MoveSpeedValueChanged(float previousValue, float newValue)
    {
        Event_StatsChanged?.Invoke();
    }
    public void OnAttacked(IAttackRange attacker, int spacialDamage = -1)
    {
        if (_isCheckDead) return;

        NetworkObjectReference netWorkRef = GetOnAttackedOwner(attacker);
        OnAttackedRpc(netWorkRef, spacialDamage);
    }


    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void OnAttackedRpc(NetworkObjectReference attackerRef, int spacialDamage = -1)
    {
        int damage = 0;
        attackerRef.TryGet(out NetworkObject attackerNGO);
        attackerNGO.TryGetComponent(out BaseStats attackerStats);

        if (spacialDamage < 0)
            damage = attackerStats.Attack;
        else
            damage = spacialDamage;
        damage = Mathf.Max(0, damage - Defence);
        Hp -= damage;
        if (Hp <= 0)
        {
            Hp = 0;
            OnDead(attackerStats);
            _isCheckDead = true;
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void OnAttackedClientRpc(int damage)
    {
        Event_Attacked?.Invoke(damage);
    }

    public NetworkObjectReference GetOnAttackedOwner(IAttackRange attacker)
    {
        if (attacker.Owner_Transform.TryGetComponent(out NetworkObject ngo))
        {
            return new NetworkObjectReference(ngo);
        }
        Debug.Log("Attacker hasn't a BaseStats");
        return default;
    }

    protected abstract void OnDead(BaseStats attacker);
}
