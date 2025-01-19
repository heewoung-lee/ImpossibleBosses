using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using UnityEditor;
using Unity.VisualScripting;
using System.Net;
using UnityEditor.PackageManager;
using UnityEngine.UIElements;

interface ILoader<TKey, TValue>
{
    Dictionary<TKey, TValue> MakeDict();
}

public class DataManager : IManagerInitializable,IManagerIResettable
{
    private const string SPREEDSHEETID = "1DV5kuhzjcNs3id8deI8Q3xFgbOWTa_pr76uSD0gNpGg";

    private List<Type> _requestDataTypes;

    private GoogleDataBaseStruct _databaseStruct;
    public Dictionary<Type,object> AllDataDict { get; } = new Dictionary<Type,object>();
    public GoogleDataBaseStruct DatabaseStruct
    {
        get
        {
            if (_databaseStruct.Equals(default(GoogleDataBaseStruct)))
            {
                _databaseStruct = new GoogleDataBaseStruct(Define.GOOGLE_CLIENT_ID, Define.GOOGLE_SECRET, Define.APPLICATIONNAME, SPREEDSHEETID);
            }
            return _databaseStruct;
        }
    }

    public void Init()
    {
        _requestDataTypes = LoadSerializableTypesFromFolder("Assets/Scripts/Data/DataType", AddSerializableAttributeType);

        //������ �ε�
        LoadDataFromGoogleSheets(_requestDataTypes);
    }

    public List<Type> LoadSerializableTypesFromFolder(string folderPath,Action<Type,List<Type>> wantTypeFilter)//Ŭ������ folderPath����� ������ Serialze��Ʈ����Ʈ�� ���� Ÿ�Ե� ��������
    {
        List<Type> pathClasses = new List<Type>();

        string[] guids = AssetDatabase.FindAssets("t:MonoScript", new[] { folderPath });

        foreach (string guid in guids)
        {
            // GUID�� ���� ���� ��θ� ������
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            // MonoScript ���� �ε�
            MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
            if (monoScript != null)
            {
                // ��ũ��Ʈ���� ���ǵ� Ŭ���� Ÿ�� ��������
                Type type = monoScript.GetClass();
                if (type != null)
                {
                    wantTypeFilter.Invoke(type, pathClasses);//���ϴ� �ֵ��� �����ؼ� ��������
                }
            }
        }
        return pathClasses;
    }
    public void AddSerializableAttributeType(Type monoScriptType,List<Type> typeList)
    {
        if (Attribute.IsDefined(monoScriptType, typeof(SerializableAttribute)))
        {
            typeList.Add(monoScriptType);
            //Debug.Log($"Find RequestType: {type.FullName}");
        }
    }


    private bool LoadAllDataFromLocal(string typeName)
    {
        TextAsset[] jsonFiles = Managers.ResourceManager.LoadAll<TextAsset>("Data");

        foreach (TextAsset jsonFile in jsonFiles)
        {
            if (typeName != GetTypeNameFromFileName(jsonFile.name))
            {
                continue;
            }
            else
            {
                AddAllDataDictFromJsonData(jsonFile.name, jsonFile.text);
                return true;
            }
        }
        return false;
    }


    private void AddAllDataDictFromJsonData(string jsonFileName, string jsonString)
    {
        string typeName = GetTypeNameFromFileName(jsonFileName);
        if (string.IsNullOrEmpty(typeName))
            return;

        Type statType = Type.GetType($"{typeName}, Assembly-CSharp");
        if (statType == null)
        {
            Debug.LogError($"Type '{typeName}' not found.");
            return;
        }

        Type keyType = FindGenericKeyType(statType);
        Type loaderType = typeof(DataToDictionary<,>).MakeGenericType(keyType, statType);

        MethodInfo method = typeof(JsonConvert).GetMethods().First(m => m.Name == "DeserializeObject" && m.IsGenericMethod);
        MethodInfo genericMethod = method.MakeGenericMethod(loaderType);
        object statData = genericMethod.Invoke(null, new object[] { jsonString });

        MethodInfo makeDicMethod = loaderType.GetMethod("MakeDict");
        object dict = makeDicMethod.Invoke(statData, null);

        AllDataDict[statType] = dict;
    }





    private void SaveDataToFile(string fileName, string jsonString)
    {
        string directoryPath = Path.Combine(Application.dataPath, "Resources/Data");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string filePath = Path.Combine(directoryPath, $"{fileName}.json");

        if (File.Exists(filePath))
        {
            string existingJson = File.ReadAllText(filePath);
            if (existingJson == jsonString)
            {
                Debug.Log($"{fileName} �����Ϳ� ���� ������ �����ϴ�.");
                return;
            }
        }
        File.WriteAllText(filePath, jsonString);
        Debug.Log($"{fileName} �����͸� ���ÿ� �����߽��ϴ�.");
    }

    public Spreadsheet GetGoogleSheetData(GoogleDataBaseStruct databaseStruct,out SheetsService service,out string spreadsheetId,bool isWrite = false)
    {
        // ���� ���������Ʈ���� ������ �ε��ϴ� ����
        // ��ȯ��: Dictionary<string, string> (sheetName, jsonString)

        // ���� ���� �� ���� ����
        try
        {
           string[] readAndWriteOption = isWrite == true ? new[] { SheetsService.Scope.Spreadsheets } : new[] { SheetsService.Scope.SpreadsheetsReadonly };
            UserCredential _credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = databaseStruct._google_Client_ID,
                    ClientSecret = databaseStruct._google_Secret
                },
                readAndWriteOption,
                "user",
                CancellationToken.None).Result;

            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _credential,
                ApplicationName = databaseStruct._applicationName
            });

            spreadsheetId = databaseStruct._spreedSheetID;

            // ���������Ʈ ��û
            Spreadsheet spreadsheet = service.Spreadsheets.Get(spreadsheetId).Execute();
            return spreadsheet;
        }
        catch (Exception error)//���� ���������Ʈ�� ������ �ȵɶ� ����ó��
        {
            Debug.Log($"Error: {error}\nNot Connetced Internet");
            UI_AlertDialog alertDialog = Managers.UI_Manager.TryGetPopupDictAndShowPopup<UI_AlertDialog>();
            alertDialog.SetText("����", "���ͳ� ������ �ȵƽ��ϴ�.");

            throw;
        }
    }

    private void LoadDataFromGoogleSheets(List<Type> requestDataTypes)
    {
        // ���� ���������Ʈ���� ������ �ε��ϴ� ����
        // ��ȯ��: Dictionary<string, string> (sheetName, jsonString)

        // ���� ���� �� ���� ����
        try
        {
            // ���������Ʈ ��û
            Spreadsheet spreadsheet = GetGoogleSheetData(DatabaseStruct, out SheetsService service,out string spreadsheetId);
            foreach (Type requestType in requestDataTypes)
            {
                Sheet sheet = null;
                for (int i = 0; i < spreadsheet.Sheets.Count; i++)
                {
                    if (GetTypeNameFromFileName(spreadsheet.Sheets[i].Properties.Title) != requestType.Name)
                        continue;
                    else
                    {
                        sheet = spreadsheet.Sheets[i];
                        break;
                    }
                }
                if (sheet != null) //�ʿ��� ������ Ÿ���� ��Ʈ�� �ִٸ� DB�� �ִ°� ����.
                {
                    string sheetName = sheet.Properties.Title;

                    string range = $"{sheetName}!A1:Z"; // �ʿ��� ���� ���� ���� �� �аڴ�.
                    SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);
                    ValueRange response = request.Execute();

                    string jsonString = ParseSheetData(response.Values);

                    if (Managers.ResourceManager.TryGetLoad($"Data/{sheetName}", out TextAsset originJsonFile))//�ִٸ�.
                    {
                        if (BinaryCheck<string>(jsonString, originJsonFile.ToString()) == false)
                        {
                            SaveDataToFile(sheetName, jsonString);//�ִµ� �ΰ��� �ٸ��ٸ� �ֽ��� ����
                        }
                    }
                    else
                    {
                        SaveDataToFile(sheetName, jsonString);//���ٸ� ����
                    }
                    AddAllDataDictFromJsonData(sheetName, jsonString);
                }
                else
                {
                    if (LoadAllDataFromLocal(requestType.Name) == false)
                    {
                        Debug.LogError($"Not Found RequestType  \"{requestType.Name}\"");
                    }
                }
            }
        }
        catch (Exception error)//���� ���������Ʈ�� ������ �ȵɶ� ����ó��
        {
            Debug.Log(error);
            Debug.Log("Load from LocalJson");
            foreach (Type requestType in requestDataTypes)
            {
                if (LoadAllDataFromLocal(requestType.Name) == false)
                {
                    Debug.LogError($"Not Found RequestType  \"{requestType.Name}\"");
                }
            }
        }
    }

    private string GetTypeNameFromFileName(string filepath)
    {
        if (string.IsNullOrEmpty(filepath))
        {
            Debug.LogError("File name is null or empty.");
            return null;
        }

        string typeName = filepath.Replace("_", "").Replace("Data", "");
        if (typeName.Length == 0)
            return null;

        return typeName;
    }

    public Type FindGenericKeyType(Type typeinfo)
    {
        Type[] TypeInterfaces = typeinfo.GetInterfaces();

        foreach (Type TypeInterface in TypeInterfaces)
        {
            if (TypeInterface.IsGenericType && TypeInterface.GetGenericTypeDefinition() == typeof(Ikey<>))
            {
                //���׸�Ÿ���� ù��° �Ű������� ������. = Ű�� �Ǵ� �Ű����� 
                return TypeInterface.GetGenericArguments()[0];
            }
        }
        return null;
    }

    private string ParseSheetData(IList<IList<object>> value)
    {
        StringBuilder jsonBuilder = new StringBuilder();

        IList<object> columns = value[0];

        jsonBuilder.Append("{\n");
        jsonBuilder.Append("\"stats\":");
        jsonBuilder.Append("[\n");
        for (int row = 1; row < value.Count; row++)
        {
            IList<object> data = value[row];
            jsonBuilder.Append("{\n");
            for (int col = 0; col < data.Count; col++)
            {
                jsonBuilder.Append("\"" + columns[col] + "\"" + ":");

                // �迭���� Ȯ���Ͽ� ����ǥ ó��
                if (data[col].ToString().StartsWith("["))
                {
                    // �迭�� ����ǥ ���� �߰�
                    jsonBuilder.Append(data[col]);
                }
                else
                {
                    // �Ϲ� ���ڿ��� ����ǥ�� ���α�
                    jsonBuilder.Append("\"" + data[col] + "\"");
                }

                if (col != data.Count - 1)
                    jsonBuilder.Append(",\n");
                else
                    jsonBuilder.Append("\n");
            }
            jsonBuilder.Append("}");
            if (row != value.Count - 1)
                jsonBuilder.Append(",\n");
        }
        jsonBuilder.Append("]");

        jsonBuilder.Append("}");

        return jsonBuilder.ToString();
    }

    private bool BinaryCheck<T>(T src, T target)
    {
        //�� ����� ���̳ʸ��� ��ȯ�ؼ� ��, �ٸ��� false ��ȯ
        BinaryFormatter formatter1 = new BinaryFormatter();
        MemoryStream stream1 = new MemoryStream();
        formatter1.Serialize(stream1, src);

        BinaryFormatter formatter2 = new BinaryFormatter();
        MemoryStream stream2 = new MemoryStream();
        formatter2.Serialize(stream2, target);

        byte[] srcByte = stream1.ToArray();
        byte[] tarByte = stream2.ToArray();

        if (srcByte.Length != tarByte.Length)
        {
            Debug.Log("Data has changed");
            return false;
        }
        for (int i = 0; i < srcByte.Length; i++)
        {
            if (srcByte[i] != tarByte[i])
            {
                Debug.Log("Data has changed");
                return false;
            }
        }
        return true;
    }

    public void Clear()
    {
        AllDataDict.Clear();
    }

}