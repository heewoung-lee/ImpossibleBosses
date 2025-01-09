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
    private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    enum SheetIndex
    {
        ID,
        PW,
        NickName
    }
    private const string USER_AUTHENTICATE_DATASHEET_NAME = "UserAuthenticateData";

    PlayerLoginInfo currentPlayerInfo;

    public PlayerLoginInfo CurrentPlayerInfo { get { return currentPlayerInfo; } }

    public PlayerLoginInfo AuthenticateUser(string userID, string userPW)
    {
        //구글 스프레드 시트에 접근해서 맞는 아이디와 패스워드를 확인한후
        //있으면 로비창으로 씬전환
        //없으면 오류메세지 출력

        Spreadsheet spreadsheet = Managers.DataManager.GetGoogleSheetData(out SheetsService service, out string spreadsheetId);
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
                    ? row[(int)SheetIndex.NickName].ToString():"";

                currentPlayerInfo = new PlayerLoginInfo()
                {
                    ID = idData,
                    Password = pwData,
                    NickName = nickNameData,
                };
            }
            if (userID == currentPlayerInfo.ID && userPW == currentPlayerInfo.Password)
            {
                Debug.Log("Login Success");
                return currentPlayerInfo;
            }

        }
        return default;
    }


    public async Task WriteToGoogleSheet(string id, string password)
    {

        Spreadsheet sheet = Managers.DataManager.GetGoogleSheetData(out SheetsService service, out string spreadsheetId,true);
        // 작성할 데이터 준비
        List<IList<object>> values = new List<IList<object>>()
        {
            new List<object> { id, password} // A열, B열, C열에 기록
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
        AppendValuesResponse response = await appendRequest.ExecuteAsync();
        Console.WriteLine("데이터가 성공적으로 추가되었습니다.");
    }

}