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
        //���� �������� ��Ʈ�� �����ؼ� �´� ���̵�� �н����带 Ȯ������
        //������ �κ�â���� ����ȯ
        //������ �����޼��� ���

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

        string range = $"{UserAthenticateData.Properties.Title}!A1:Z"; // �ʿ��� ���� ���� ���� �� �аڴ�.
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
            // �� row�� ���� �����Ͱ� �ִ��� Ȯ��
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
        // �ۼ��� ������ �غ�
        List<IList<object>> values = new List<IList<object>>()
        {
            new List<object> { id, password} // A��, B��, C���� ���
        };

        ValueRange valueRange = new ValueRange
        {
            Values = values
        };

        // ���� ����: ��Ʈ �̸��� ������ ����
        string range = $"{USER_AUTHENTICATE_DATASHEET_NAME}!A1:Z"; // ��Ʈ �̸��� ���� (A��~C��)

        // ������Ʈ ��û ����
        SpreadsheetsResource.ValuesResource.AppendRequest appendRequest = service.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
        appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

        // ��û ����
        AppendValuesResponse response = await appendRequest.ExecuteAsync();
        Console.WriteLine("�����Ͱ� ���������� �߰��Ǿ����ϴ�.");
    }

}