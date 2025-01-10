using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Android.Gradle.Manifest;
using System.Threading.Tasks;

public struct PlayerLoginInfo
{
    public string ID;
    public string Password;
    public string NickName;
}

public class LogInManager
{
    private const string SPREEDSHEETID = "1SKhi41z1KRfHI6KwhQ2iM3mSjgLZKXw7_VopoIEZYNQ";

    private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    enum SheetIndex
    {
        ID,
        PW,
        NickName
    }
    private const string USER_AUTHENTICATE_DATASHEET_NAME = "UserAuthenticateData";

    private GoogleDataBaseStruct _googleDataBaseStruct;

    GoogleDataBaseStruct GoogleDataBaseStruct
    {
        get
        {
            if(_googleDataBaseStruct.Equals(default(GoogleDataBaseStruct)))
            {
                _googleDataBaseStruct = new GoogleDataBaseStruct(Define.GOOGLE_CLIENT_ID, Define.GOOGLE_SECRET, Define.APPLICATIONNAME, SPREEDSHEETID);
            }
            return _googleDataBaseStruct;
        }
    }



    PlayerLoginInfo currentPlayerInfo;

    public PlayerLoginInfo CurrentPlayerInfo { get { return currentPlayerInfo; } }

    public PlayerLoginInfo AuthenticateUserCommon(string userID, string userPW=null)
    {
        //구글 스프레드 시트에 접근해서 맞는 아이디와 패스워드를 확인한후
        //있으면 로비창으로 씬전환
        //없으면 오류메세지 출력
        Spreadsheet spreadsheet = Managers.DataManager.GetGoogleSheetData(GoogleDataBaseStruct, out SheetsService service, out string spreadsheetId);
        Sheet UserAthenticateData = null;

        foreach (Sheet sheet in spreadsheet.Sheets)
        {
            if (sheet.Properties.Title == USER_AUTHENTICATE_DATASHEET_NAME)
            {
                UserAthenticateData = sheet;
                break;
            }
        }

        if (UserAthenticateData == null)
        {
            Debug.LogError($"Not Found {USER_AUTHENTICATE_DATASHEET_NAME} Sheet");
            return default;
        }

        string range = $"{UserAthenticateData.Properties.Title}!A1:Z"; // 필요한 범위 지정 전부 다 읽겠다.
        SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);
        ValueRange response = request.Execute();


        if (response == null)
        {
            Debug.LogError("There is Empty UserAuthenticateData");
            return default;
        }

        for (int rowIndex = 1; rowIndex < response.Values.Count; rowIndex++)
        {
            IList<object> row = response.Values[rowIndex];
            // 이 row에 실제 데이터가 있는지 확인
            if (row.Count > 1)
            {
                string idData = row[(int)SheetIndex.ID].ToString();
                string pwData = row[(int)SheetIndex.PW].ToString();
                string nickNameData = idData != null && pwData != null && row.Count > 2
                    ? row[(int)SheetIndex.NickName].ToString() : "";

                currentPlayerInfo = new PlayerLoginInfo()
                {
                    ID = idData,
                    Password = pwData,
                    NickName = nickNameData,
                };
            }

            if(userPW != null)
            {
                if (userID == currentPlayerInfo.ID && userPW == currentPlayerInfo.Password)
                {
                    Debug.Log("DB Has ID And PW");
                    return currentPlayerInfo;
                }
            }
            else
            {
                if (userID == currentPlayerInfo.ID)
                {
                    Debug.Log("DB Has ID");
                    return currentPlayerInfo;
                }
            }
           

        }
        return default;
    }




    public PlayerLoginInfo AuthenticateUser(string userID, string userPW)
    {
        return AuthenticateUserCommon(userID, userPW);
    }
    public PlayerLoginInfo AuthenticateUser(string userID)
    {
        return AuthenticateUserCommon(userID);
    }

    public async Task<(bool,string)> WriteToGoogleSheet(string id, string password)
    {

        Spreadsheet sheet = Managers.DataManager.GetGoogleSheetData(GoogleDataBaseStruct, out SheetsService service, out string spreadsheetId,true);

        PlayerLoginInfo isIDInDatabase = AuthenticateUser(id);

        if(isIDInDatabase.Equals(default(PlayerLoginInfo)) == false)
        {
            Debug.Log("이미 있는 ID 입니다");
            return (false, "이미 있는 ID 입니다");
        }

        // 작성할 데이터 준비
        List<IList<object>> values = new List<IList<object>>()
        {
            new List<object> { id, password} 
        };

        ValueRange valueRange = new ValueRange
        {
            Values = values
        };

        // 범위 설정: 시트 이름과 범위를 지정
        string range = $"{USER_AUTHENTICATE_DATASHEET_NAME}!A1:Z"; // 시트 이름과 범위 (A열~C열)

        // 업데이트 요청 생성
        SpreadsheetsResource.ValuesResource.AppendRequest appendRequest = service.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
        appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

        // 요청 실행
        try
        {
            AppendValuesResponse response = await appendRequest.ExecuteAsync();
        }
        catch(Exception ex)
        {
            Debug.Log($"{ex}오류가 발생했습니다");
            return (false, "DB를 쓰는중 오류가 발생했습니다.");
        }
        return (true, "회원가입을 축하드립니다.");
    }

}