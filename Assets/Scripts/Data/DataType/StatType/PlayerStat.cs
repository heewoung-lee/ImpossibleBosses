using System;

namespace Data.DataType.StatType
{
    [Serializable]
    public struct PlayerStat : IKey<int>
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
}
