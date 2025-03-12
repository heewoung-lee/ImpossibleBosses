using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
[DisallowMultipleComponent]
public abstract class BaseStats : NetworkBehaviour, IDamageable
{
    private bool _isCheckDead = false;

    public Action<int,int> Event_Attacked; //현재 HP가 바로 안넘어와서 두번쨰 매개변수에 현재 HP값 전달
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
    //TODO: 클라이언트의 HP바가 NaN으로 나오는 이유는 스탯 밸류가 초기화 되기도 전에 호출을 해서 0/0이 되어
    //UI의 값이 NaN으로 나왔던거임.해결은 모든 스탯이 초기화가 완료되면, 호출할것

    private void HpValueChanged(int previousValue, int newValue)
    {
        Event_StatsChanged?.Invoke();
        int damage = previousValue - newValue;
        if (damage > 0)
        {
            if(IsHost)
            OnAttackedClientRpc(damage, newValue);
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
    public void OnAttackedClientRpc(int damage,int currentHp)
    {
        Event_Attacked?.Invoke(damage,currentHp);
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
