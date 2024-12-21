using System;

[Serializable]
public struct MonsterStat : Ikey<int>
{
    public int monsterID;
    public int hp;
    public int attack;
    public int exp;
    public int defence;
    public float speed;

    public int Key => monsterID;
}
