using System;

[Serializable]
public struct BossStat : Ikey<int>
{
    public int bossID;
    public int hp;
    public int attack;
    public int defence;
    public float speed;
    public float viewAngle;
    public float viewDistance;

    public int Key => bossID;
}
