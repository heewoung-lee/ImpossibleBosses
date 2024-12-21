
using Newtonsoft.Json;
using System;
using System.Data;
using System.IO;
using UnityEngine;

public static class DataUtil
{
    public static DataTable GetDataTable(string fileName, string tableName)
    {
        UnityEngine.Object obj = Managers.ResourceManager.Load<UnityEngine.Object>(fileName);
        string value = (obj as TextAsset).ToString();
        DataTable data = JsonConvert.DeserializeObject<DataTable>(value);
        data.TableName = tableName;

        return data;
    }

    public static DataTable GetDataTable(FileInfo info)
    {
        string fileName = Path.GetFileNameWithoutExtension(info.Name);
        string path = string.Concat("Data/", fileName);
        string value = string.Empty;
        try
        {
            value = Managers.ResourceManager.Load<TextAsset>(path).ToString();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }

        DataTable data = JsonConvert.DeserializeObject<DataTable>(value);
        data.TableName = fileName;

        return data;
    }
    public static void SetObjectFile<T>(string key, T data)
    {
        string value = JsonConvert.SerializeObject(data);
        File.WriteAllText(Application.dataPath + "/Resources/Data/" + key + ".json", value);
    }
}