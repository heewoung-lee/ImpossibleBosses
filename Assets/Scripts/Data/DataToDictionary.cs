using System.Collections.Generic;
using GameManagers;

namespace Data
{
    public class DataToDictionary<TKey, TStat> : ILoader<TKey, TStat> where TStat : IKey<TKey>
    {
        public List<TStat> stats = new List<TStat>();

        public Dictionary<TKey, TStat> MakeDict()
        {
            Dictionary<TKey, TStat> dict = new Dictionary<TKey, TStat>();
            foreach (TStat stat in stats)
            {
                dict[stat.Key] = stat;
            }
            return dict;
        }
    }
}
