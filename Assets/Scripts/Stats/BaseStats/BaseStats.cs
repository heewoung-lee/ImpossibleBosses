using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
[DisallowMultipleComponent]
public abstract class BaseStats : NetworkBehaviour, IDamageable
{
    private bool _isCheckDead = false;

    public Action<int> Event_Attacked;
    public Action Event_StatsLoaded;
    public Action Event_StatsChanged;
    public Action Done_Base_Stats_Loading;

    public NetworkVariable<int> playerHpValue = new NetworkVariable<int>
        (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> playerMaxHpValue = new NetworkVariable<int>
        (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> playerAttackValue = new NetworkVariable<int>
        (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> playerDefenceValue = new NetworkVariable<int>
       (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> playerMoveSpeedValue = new NetworkVariable<float>
       (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public int Hp {
        get => playerHpValue.Value;
        protected set
        {
            if (IsServer)
            {
                playerHpValue.Value = Mathf.Clamp(value, 0, playerMaxHpValue.Value);
            }
            else
            {
                RequestHpValueServerRpc(Mathf.Clamp(value, 0, playerMaxHpValue.Value));
            }
        }
    }
    public int MaxHp
    {
        get => playerMaxHpValue.Value;
        protected set
        {
            if (IsServer)
            {
                playerMaxHpValue.Value = Mathf.Clamp(value, 0, int.MaxValue);
            }
            else
            {
                RequestMaxHpValueServerRpc(Mathf.Clamp(value, 0, int.MaxValue));
            }
        }
    }
    public int Attack
    {
        get => playerAttackValue.Value;
        protected set
        {
            if (IsServer)
            {
                playerAttackValue.Value = Mathf.Clamp(value, 0, int.MaxValue);
            }
            else
            {
                RequestAttackValueServerRpc(Mathf.Clamp(value, 0, int.MaxValue));
            }
        }
    }
    public int Defence
    {
        get => playerDefenceValue.Value;
        protected set
        {
            if (IsServer)
            {
                playerDefenceValue.Value = Mathf.Clamp(value, 0, int.MaxValue);
            }
            else
            {
                RequestDefenceValueServerRpc(Mathf.Clamp(value, 0, int.MaxValue));
            }
        }
    }
    public float MoveSpeed
    {
        get => playerMoveSpeedValue.Value;
        protected set
        {
            if (IsSpawned == false)
                return;
            if (IsServer)
            {
                playerMoveSpeedValue.Value = Mathf.Clamp(value, 0, float.MaxValue);
            }
            else
            {
                RequestMoveSpeedValueServerRpc(Mathf.Clamp(value, 0, float.MaxValue));
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestHpValueServerRpc(int value)
    {
        playerHpValue.Value = value;
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestMaxHpValueServerRpc(int value)
    {
        playerMaxHpValue.Value = value;
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestAttackValueServerRpc(int value)
    {
        playerAttackValue.Value = value;
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestDefenceValueServerRpc(int value)
    {
        playerDefenceValue.Value = value;
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestMoveSpeedValueServerRpc(float value)
    {
        playerMoveSpeedValue.Value = value;
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
        playerDefenceValue.OnValueChanged -= DefenceValueChanged;
        playerMoveSpeedValue.OnValueChanged += MoveSpeedValueChanged;
        UpdateStat();
    }

    private void HpValueChanged(int previousValue, int newValue)
    {
        Event_StatsChanged?.Invoke();
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
        OnAttackedServerRpc(netWorkRef, spacialDamage);
    }


    [ServerRpc]
    public void OnAttackedServerRpc(NetworkObjectReference attackerRef, int spacialDamage = -1)
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
        OnAttackedClientRpc(damage);
        if (Hp <= 0)
        {
            Hp = 0;
            OnDead(attackerStats);
            _isCheckDead = true;
        }
    }
    [ClientRpc]
    public void OnAttackedClientRpc(int damage)
    {
        Event_Attacked?.Invoke(damage);
    }

    public NetworkObjectReference GetOnAttackedOwner(IAttackRange attacker)
    {
        if(attacker.Owner_Transform.TryGetComponent(out NetworkObject ngo))
        {
            return new NetworkObjectReference(ngo);
        }
        Debug.Log("Attacker hasn't a BaseStats");
        return default;
    }



    protected abstract void OnDead(BaseStats attacker);
}
