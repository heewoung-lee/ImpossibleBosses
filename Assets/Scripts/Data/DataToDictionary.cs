using System.Collections.Generic;

public class DataToDictionary<TKey, TStat> : ILoader<TKey, TStat> where TStat : Ikey<TKey>
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
