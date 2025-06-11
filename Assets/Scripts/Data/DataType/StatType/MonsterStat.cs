using System;

namespace Data.DataType.StatType
{
    [Serializable]
    public struct MonsterStat : IKey<int>
    {
        public int monsterID;
        public int hp;
        public int attack;
        public int exp;
        public int defence;
        public float speed;

        public int Key => monsterID;
    }
}
