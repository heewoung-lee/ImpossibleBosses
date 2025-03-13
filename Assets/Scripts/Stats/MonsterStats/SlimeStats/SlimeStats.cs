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
        MonsterStat stat = _statDict[(int)SlimeID];
        CharacterBaseStat basestat = new CharacterBaseStat(stat.hp, stat.hp, stat.attack, stat.defence,stat.speed);
        SetPlayerBaseStatRpc(basestat);
        _exp = stat.exp;
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
        UpdateStat();
    }
}
