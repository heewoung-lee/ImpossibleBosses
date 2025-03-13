using System;
using Unity.Netcode;

[Serializable]
public struct PlayerStat : Ikey<int>
{
    public int level;
    public int hp;
    public int attack;
    public int xpRequired;
    public int defence;
    public float speed;
    public float viewAngle;
    public float viewDistance;

    public int Key => level;
}