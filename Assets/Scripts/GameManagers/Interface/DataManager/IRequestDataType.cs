
using System;
using System.Collections.Generic;

namespace GameManagers.Interface.DataManager
{
    public interface IRequestDataType //Scene Context
    {
        public IList<Type> LoadSerializableTypesFromFolder(string folderPath, Action<Type,List<Type>> wantTypeFilter);
    }
}
