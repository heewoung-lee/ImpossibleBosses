using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using Data;
using GameManagers.Interface.DataManager;
using GameManagers.Interface.GoogleAuthLogin;
using GameManagers.Interface.Resources_Interface;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Util;
using Zenject;

namespace GameManagers
{
    interface ILoader<TKey, TValue>
    {
        Dictionary<TKey, TValue> MakeDict();
    }

    public class DataManager : IInitializable
    {
        [Inject] IResourcesLoader _resourcesLoader;
        [Inject] IGoogleAuthLoginLoader _googleAuthLogin;
        [Inject] ISpreedSheetID _spreedSheetID;

        private List<Type> _requestDataTypes;
        private Dictionary<string, Type> _loadDataTypetoDict;

        private GoogleDataBaseStruct _databaseStruct;
        public Dictionary<Type,object> AllDataDict { get; } = new Dictionary<Type,object>();
        public GoogleDataBaseStruct DatabaseStruct
        {
            get
            {
                if (_databaseStruct.Equals(default(GoogleDataBaseStruct)))
                {
                    TextAsset[] jsonTexts = _googleAuthLogin.LoadGoogleAuthJsonFiles();
                    GoogleLoginWrapper googleLoginData = _googleAuthLogin.ParseJsontoGoogleAuth(jsonTexts);
                    _databaseStruct = new GoogleDataBaseStruct(googleLoginData.installed.client_id, googleLoginData.installed.client_secret, Define.Applicationname, _spreedSheetID.Spreedsheetid);
                }
                return _databaseStruct;
            }
        }

        public void Initialize()
        {
            _requestDataTypes = LoadSerializableTypesFromFolder("Assets/Scripts/Data/DataType", AddSerializableAttributeType);

            _loadDataTypetoDict = new Dictionary<string, Type>();
            foreach (Type typeData in _requestDataTypes)
            {
                _loadDataTypetoDict.Add(typeData.Name, typeData);
            }
            //데이터 로드
            LoadDataFromGoogleSheets(_requestDataTypes);
        }

        public List<Type> LoadSerializableTypesFromFolder(string folderPath,Action<Type,List<Type>> wantTypeFilter)
        {
            List<Type> pathClasses = new List<Type>();

            string[] guids = AssetDatabase.FindAssets("t:MonoScript", new[] { folderPath });

            foreach (string guid in guids)
            {
                // GUID를 통해 에셋 경로를 가져옴
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                // MonoScript 에셋 로드
                MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
                if (monoScript != null)
                {
                    // 스크립트에서 정의된 클래스 타입 가져오기
                    Type type = monoScript.GetClass();
                    if (type != null)
                    {
                        wantTypeFilter.Invoke(type, pathClasses);//원하는 객체를 필터해서 가져오기
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
            TextAsset[] jsonFiles = _resourcesLoader.LoadAll<TextAsset>("Data");

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
            
            //Type statType = Type.GetType($"{typeName}, Assembly-CSharp");
            //6.11일 수정 타입 클래스들의 네임스페이스를 씌우니 GetType에서 정확한 네임스페이스를 요구함.
            //따라서 Init부분에 필요한 로드 타입을 미리 캐싱 해놓고 현재 메소드에서 로드한 json의 타입이름 들만 가져와
            //캐싱되어있는 딕셔너리를 가져옴
            Type statType = null;

            if (_loadDataTypetoDict.TryGetValue(typeName, out Type loadType) == true)
            {
                statType = _loadDataTypetoDict[typeName];
            }
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
                    Debug.Log($"{fileName} 데이터에 변경 사항이 없습니다.");
                    return;
                }
            }
            File.WriteAllText(filePath, jsonString);
            Debug.Log($"{fileName} 데이터를 로컬에 저장했습니다.");
        }

        public Spreadsheet GetGoogleSheetData(GoogleDataBaseStruct databaseStruct,out SheetsService service,out string spreadsheetId,bool isWrite = false)
        {
            // 구글 스프레드시트에서 데이터 로드하는 로직
            // 반환값: Dictionary<string, string> (sheetName, jsonString)

            // 구글 인증 및 서비스 생성
            try
            {
                string[] readAndWriteOption;
                string tokenID;
                if (isWrite == true)
                {
                    readAndWriteOption = new[] { SheetsService.Scope.Spreadsheets };
                    tokenID = "WriteUser";
                }
                else
                {
                    readAndWriteOption = new[] { SheetsService.Scope.SpreadsheetsReadonly };
                    tokenID = "ReadUser";
                }
                UserCredential _credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets
                    {
                        ClientId = databaseStruct.GoogleClientID,
                        ClientSecret = databaseStruct.GoogleSecret
                    },
                    readAndWriteOption,
                    tokenID,
                    CancellationToken.None).Result;

                service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = _credential,
                    ApplicationName = databaseStruct.ApplicationName
                });

                spreadsheetId = databaseStruct.SpreedSheetID;

                // 스프레드시트 요청
                Spreadsheet spreadsheet = service.Spreadsheets.Get(spreadsheetId).Execute();
                return spreadsheet;
            }
            catch
            {
                throw;
            }
        }

        private void LoadDataFromGoogleSheets(List<Type> requestDataTypes)
        {
            // 구글 스프레드시트에서 데이터 로드하는 로직
            // 반환값: Dictionary<string, string> (sheetName, jsonString)

            // 구글 인증 및 서비스 생성
            try
            {
                // 스프레드시트 요청
                Spreadsheet spreadsheet = GetGoogleSheetData(DatabaseStruct, out SheetsService service,out string spreadsheetId);
                foreach (Type requestType in requestDataTypes)
                {
                    Sheet sheet = null;
                    for (int i = 0; i < spreadsheet.Sheets.Count; i++)
                    {
                        if (GetTypeNameFromFileName(spreadsheet.Sheets[i].Properties.Title) != requestType.Name)
                        {
                            continue;
                        }
                        else
                        {
                            sheet = spreadsheet.Sheets[i];
                            break;
                        }
                    }
                    if (sheet != null) //필요한 데이터 타입의 시트가 있다면 DB에 있는걸 쓴다.
                    {
                        string sheetName = sheet.Properties.Title;

                        string range = $"{sheetName}!A1:Z"; // 필요한 범위 지정 전부 다 읽겠다.
                        SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);
                        ValueRange response = request.Execute();

                        string jsonString = ParseSheetData(response.Values);

                        if (_resourcesLoader.TryGetLoad($"Data/{sheetName}", out TextAsset originJsonFile))//있다면.
                        {
                            if (BinaryCheck<string>(jsonString, originJsonFile.ToString()) == false)
                            {
                                SaveDataToFile(sheetName, jsonString);//있는데 두개가 다르다면 최신을 저장
                            }
                        }
                        else
                        {
                            SaveDataToFile(sheetName, jsonString);//없다면 저장
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
            catch (Exception error)//구글 스프레드시트에 연결이 안될때 에러처리
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
            Type[] typeinterfaces = typeinfo.GetInterfaces();

            foreach (Type typeInterface in typeinterfaces)
            {
                if (typeInterface.IsGenericType && typeInterface.GetGenericTypeDefinition() == typeof(IKey<>))
                {
                    //제네릭타입의 첫번째 매개변수를 던진다. = 키가 되는 매개변수 
                    return typeInterface.GetGenericArguments()[0];
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

                    // 배열인지 확인하여 따옴표 처리
                    if (data[col].ToString().StartsWith("["))
                    {
                        // 배열은 따옴표 없이 추가
                        jsonBuilder.Append(data[col]);
                    }
                    else
                    {
                        // 일반 문자열은 따옴표로 감싸기
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
            //두 대상을 바이너리로 변환해서 비교, 다르면 false 반환
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
}