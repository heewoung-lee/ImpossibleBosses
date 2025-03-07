public class SlimeStats : MonsterStats
{
    Define.MonsterID SlimeID;
    int _exp;

    protected override void SetStats()
    {
        MonsterStat stat = _statDict[(int)SlimeID];
        Hp = stat.hp;
        MaxHp = stat.hp;
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

        Managers.RelayManager.DeSpawn_NetWorkOBJ(gameObject);
    }

    protected override void StartInit()
    {
        base.StartInit();
        SlimeID = Define.MonsterID.Slime;
    }
}
