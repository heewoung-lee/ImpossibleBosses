using Unity.Netcode;

public class SlimeStats : MonsterStats
{
    Define.MonsterID SlimeID;
    int _exp;


    protected override void AwakeInit()
    {
        base.AwakeInit();
        SlimeID = Define.MonsterID.Slime;
    }
    protected override void SetStats()
    {
        if(_statDict == null)
        {
            Done_Base_Stats_Loading -= SetStats;
            Done_Base_Stats_Loading += SetStats;
            return;
        }

        MonsterStat stat = _statDict[(int)SlimeID];
        MaxHp = stat.hp;
        Hp = stat.hp;
        Attack = stat.attack;
        _exp = stat.exp;
        Defence = stat.defence;
        MoveSpeed = stat.speed;
    }

    protected override void OnDead(BaseStats attacker)
    {
        if (attacker.TryGetComponent(out PlayerStats playerStat))
        {
            playerStat.Exp += _exp;
        }

        if (gameObject.TryGetComponent(out NetworkObject ngo))
        {
            ulong networkObjectID = ngo.NetworkObjectId;
            Managers.RelayManager.DeSpawn_NetWorkOBJ(networkObjectID);
        }
    }

    protected override void StartInit()
    {
        base.StartInit();
    }
}
