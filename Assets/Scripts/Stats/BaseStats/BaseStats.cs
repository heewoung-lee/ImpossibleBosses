using System;
using Unity.Netcode;
using UnityEngine;

namespace Stats.BaseStats
{
    public struct CharacterBaseStat : INetworkSerializable
    {
        public int MaxHp;
        public int Hp;
        public int Attack;
        public int Defence;
        public float Speed;
        public static CharacterBaseStat operator -(CharacterBaseStat firstValue, CharacterBaseStat SecondValue)
        {
            return new CharacterBaseStat
            {
                MaxHp = firstValue.MaxHp - SecondValue.MaxHp,
                Hp = firstValue.Hp - SecondValue.Hp,
                Attack = firstValue.Attack - SecondValue.Attack,
                Defence = firstValue.Defence - SecondValue.Defence,
                Speed = firstValue.Speed - SecondValue.Speed
            };
        }
        public CharacterBaseStat(int hp, int maxHp, int attack, int defence, float speed)
        {
            this.Hp = hp;
            this.MaxHp = maxHp;
            this.Attack = attack;
            this.Defence = defence;
            this.Speed = speed;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Hp);
            serializer.SerializeValue(ref MaxHp);
            serializer.SerializeValue(ref Attack);
            serializer.SerializeValue(ref Defence);
            serializer.SerializeValue(ref Speed);
        }
    }
    [DisallowMultipleComponent]
    public abstract class BaseStats : NetworkBehaviour, IDamageable
    {
        private Action<int, int> _eventAttacked; //현재 HP가 바로 안넘어와서 두번 매개변수에 현재 HP값 전달
        private Action<CharacterBaseStat> _doneBaseStatsLoading;

        private Action<int, int> _currentHpValueChangedEvent;
        private Action<int, int> _maxHpValueChangedEvent;
        private Action<int, int> _attackValueChangedEvent;
        private Action<int, int> _defenceValueChangedEvent;
        private Action<float, float> _moveSpeedValueChangedEvent;
        private Action<bool, bool> _isDeadValueChagneEvent;



        public event Action<int, int> EventAttacked
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _eventAttacked, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _eventAttacked, value);
            }

        }
        public event Action<CharacterBaseStat> DoneBaseStatsLoading
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _doneBaseStatsLoading, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _doneBaseStatsLoading, value);
            }
        }
    
        public event Action<int,int> CurrentHpValueChangedEvent
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _currentHpValueChangedEvent, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _currentHpValueChangedEvent, value);
            }
        }
        public event Action<int,int> MaxHpValueChangedEvent
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _maxHpValueChangedEvent, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _maxHpValueChangedEvent, value);
            }
        }
        public event Action<int,int> AttackValueChangedEvent
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _attackValueChangedEvent, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _attackValueChangedEvent, value);
            }
        }
        public event Action<int,int> DefenceValueChangedEvent
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _defenceValueChangedEvent, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _defenceValueChangedEvent, value);
            }
        }
        public event Action<float,float> MoveSpeedValueChangedEvent
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _moveSpeedValueChangedEvent, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _moveSpeedValueChangedEvent, value);
            }
        }
        public event Action<bool,bool> IsDeadValueChagneEvent
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _isDeadValueChagneEvent, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _isDeadValueChagneEvent, value);
            }
        }

        private NetworkVariable<CharacterBaseStat> _characterBaseStatValue = new NetworkVariable<CharacterBaseStat>
            (new CharacterBaseStat(0, 0, 0, 0, 0f), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private NetworkVariable<int> _characterHpValue = new NetworkVariable<int>
            (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private NetworkVariable<int> _characterMaxHpValue = new NetworkVariable<int>
            (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private NetworkVariable<int> _characterAttackValue = new NetworkVariable<int>
            (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private NetworkVariable<int> _characterDefenceValue = new NetworkVariable<int>
            (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private NetworkVariable<float> _characterMoveSpeedValue = new NetworkVariable<float>
            (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private NetworkVariable<bool> _isDeadValue = new NetworkVariable<bool>
            (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);




        public CharacterBaseStat CharacterBaseStats
        {
            get => _characterBaseStatValue.Value;
            protected set
            {
                SetPlayerBaseStatRpc(value);
            }
        }
        [Rpc(SendTo.Server)]
        public void SetPlayerBaseStatRpc(CharacterBaseStat baseStats, RpcParams rpcParams = default)
        {
            _characterBaseStatValue.Value = baseStats;
        }
        public int Hp
        {
            get => _characterHpValue.Value;
            protected set
            {
                PlayerHpValueChangedRpc(value);
            }
        }
        [Rpc(SendTo.Server)]
        public void PlayerHpValueChangedRpc(int value)
        {
            _characterHpValue.Value = Mathf.Clamp(value, 0, MaxHp);
        }



        public int MaxHp
        {
            get => _characterMaxHpValue.Value;
            protected set
            {
                PlayerMaxHpValueChangedRpc(value);
            }
        }
        [Rpc(SendTo.Server)]
        public void PlayerMaxHpValueChangedRpc(int value)
        {
            _characterMaxHpValue.Value = Mathf.Clamp(value, 0, int.MaxValue);
        }



        public int Attack
        {
            get => _characterAttackValue.Value;
            protected set
            {
                PlayerAttackValueChangedRpc(value);
            }
        }
        [Rpc(SendTo.Server)]
        public void PlayerAttackValueChangedRpc(int value)
        {
            _characterAttackValue.Value = Mathf.Clamp(value, 0, int.MaxValue);
        }



        public int Defence
        {
            get => _characterDefenceValue.Value;
            protected set
            {
                PlayerDefenceValueChangedRpc(value);
            }
        }
        [Rpc(SendTo.Server)]
        public void PlayerDefenceValueChangedRpc(int value)
        {
            _characterDefenceValue.Value = Mathf.Clamp(value, 0, int.MaxValue);
        }



        public float MoveSpeed
        {
            get => _characterMoveSpeedValue.Value;
            protected set
            {
                PlayeMoveSpeedValueChangedRpc(value);
            }
        }
        [Rpc(SendTo.Server)]
        public void PlayeMoveSpeedValueChangedRpc(float value)
        {
            _characterMoveSpeedValue.Value = Mathf.Clamp(value, 0, float.MaxValue);
        }

        public bool IsDead
        {
            get => _isDeadValue.Value;
            protected set
            {
                IsDeadValueChangedRpc(value);
            }   
        }
        [Rpc(SendTo.Server)]
        public void IsDeadValueChangedRpc(bool value)
        {
            _isDeadValue.Value = value;
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
            _characterBaseStatValue.OnValueChanged += PlayerValueChanged;
            _characterHpValue.OnValueChanged += HpValueChanged;
            _characterMaxHpValue.OnValueChanged += MaxHpValueChanged;
            _characterAttackValue.OnValueChanged += AttackValueChanged;
            _characterDefenceValue.OnValueChanged += DefenceValueChanged;
            _characterMoveSpeedValue.OnValueChanged += MoveSpeedValueChanged;
            _isDeadValue.OnValueChanged += IsDeadValueChange;
        }
        private void PlayerValueChanged(CharacterBaseStat previousValue, CharacterBaseStat newValue)
        {
            CharacterBaseStat addValue = newValue - previousValue;

            MaxHp += addValue.MaxHp;
            Hp = newValue.Hp;
            Attack += addValue.Attack;
            Defence += addValue.Defence;
            MoveSpeed += addValue.Speed;

            if (IsOwner)
                DoneInitalizeCharacterBaseStatRpc(newValue);
        }
        private void HpValueChanged(int previousValue, int newValue)
        {
            _currentHpValueChangedEvent?.Invoke(previousValue,newValue);
            int damage = previousValue - newValue;
            if (damage > 0)
            {
                if (IsHost)
                {
                    OnAttackedClientRpc(damage, newValue);
                }
            }
        }
        private void MaxHpValueChanged(int previousValue, int newValue)
        {
            _maxHpValueChangedEvent?.Invoke(previousValue, newValue);
        }
        private void AttackValueChanged(int previousValue, int newValue)
        {
            _attackValueChangedEvent?.Invoke(previousValue, newValue);
        }
        private void DefenceValueChanged(int previousValue, int newValue)
        {
            _defenceValueChangedEvent?.Invoke(previousValue,newValue);
        }
        private void MoveSpeedValueChanged(float previousValue, float newValue)
        {
            _moveSpeedValueChangedEvent?.Invoke(previousValue,newValue);
        }
        private void IsDeadValueChange(bool previousValue, bool newValue)
        {
            _isDeadValueChagneEvent?.Invoke(previousValue,newValue);
        }


        public void OnAttacked(IAttackRange attacker, int spacialDamage = -1)
        {
            if (_isDeadValue.Value == true) return;

            NetworkObjectReference netWorkRef = TryGetOnAttackedOwner(attacker);
            OnAttackedRpc(netWorkRef, spacialDamage);
        }


        [Rpc(SendTo.Server, RequireOwnership = false)]
        public void OnAttackedRpc(NetworkObjectReference attackerRef, int spacialDamage = -1)
        {
            ulong ownetClientId = OwnerClientId;

            int damage = 0;
            attackerRef.TryGet(out NetworkObject attackerNgo);
            attackerNgo.TryGetComponent(out BaseStats attackerStats);

            if (spacialDamage < 0)
                damage = attackerStats.Attack;
            else
                damage = spacialDamage;
            damage = Mathf.Max(0, damage - Defence);
            Hp -= damage;
            if (Hp <= 0)
            {
                Hp = 0;
                OnDeadRpc(attackerRef,RpcTarget.Single(ownetClientId, RpcTargetUse.Temp));
                _isDeadValue.Value = true;
            }
        }//서버니깐 서버에서 내가 죽으면 누구한테 죽었는지 죽었다고 호출하기 


        [Rpc(SendTo.SpecifiedInParams)]
        public void OnDeadRpc(NetworkObjectReference attackerRef, RpcParams rpcParams = default)
        {
            attackerRef.TryGet(out NetworkObject attackerNgo);
            attackerNgo.TryGetComponent(out BaseStats attackerStats);
            OnDead(attackerStats);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void OnAttackedClientRpc(int damage, int currentHp)
        {
            _eventAttacked?.Invoke(damage, currentHp);
        }


        [Rpc(SendTo.Owner)]
        public void DoneInitalizeCharacterBaseStatRpc(CharacterBaseStat stat) //UI가 이벤트를 걸기도 전에 실행이 되어버린다.
        {
            _doneBaseStatsLoading?.Invoke(stat);
        }

        public NetworkObjectReference TryGetOnAttackedOwner(IAttackRange attacker)
        {
            if (attacker.OwnerTransform.TryGetComponent(out NetworkObject ngo))
            {
                return new NetworkObjectReference(ngo);
            }
            Debug.Log("Attacker hasn't a BaseStats");
            return default;
        }



        protected abstract void OnDead(BaseStats attacker);
    }
}