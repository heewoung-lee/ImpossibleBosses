using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Android.Gradle.Manifest;
using System.Threading.Tasks;
using static UnityEngine.Rendering.DebugUI;

public struct PlayerLoginInfo
{
    public string ID;
    public string Password;
    public string NickName;
    public int RowNumber;
}

public class LogInManager
{
    private const string SPREEDSHEETID = "1SKhi41z1KRfHI6KwhQ2iM3mSjgLZKXw7_VopoIEZYNQ";

    private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    enum SheetIndex
    {
        ID,
        PW,
        NickName,
        RowNumber
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

    public PlayerLoginInfo AuthenticateUserCommon(Func<PlayerLoginInfo, bool> action)
    {
        //���� �������� ��Ʈ�� �����ؼ� �´� ���̵�� �н����带 Ȯ������
        //������ �κ�â���� ����ȯ
        //������ �����޼��� ���
        Spreadsheet spreadsheet = Managers.DataManager.GetGoogleSheetData(GoogleDataBaseStruct, out SheetsService service, out string spreadsheetId);
        Sheet UserAthenticateData = null;
        bool ischeckSamePlayerLoginfo = false;
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
                    ? row[(int)SheetIndex.NickName].ToString() : "";

                currentPlayerInfo = new PlayerLoginInfo()
                {
                    ID = idData,
                    Password = pwData,
                    NickName = nickNameData,
                    RowNumber = rowIndex,
                };
            }
            ischeckSamePlayerLoginfo = action.Invoke(currentPlayerInfo);

            if (ischeckSamePlayerLoginfo == false)
                continue;
            else
                return currentPlayerInfo;
        }
        return default;
    }




    public PlayerLoginInfo AuthenticateUser(string userID, string userPW)
    {
        return AuthenticateUserCommon((currentPlayerInfo) =>
        {
            if (userID == currentPlayerInfo.ID && userPW == currentPlayerInfo.Password)
            {
                Debug.Log("DB Has ID And PW");
                return true;
            }
            return false;
        });
    }
    public PlayerLoginInfo AuthenticateUser(string userID)
    {
        return AuthenticateUserCommon((currentPlayerInfo) =>
        {
            if (userID == currentPlayerInfo.ID)
            {
                Debug.Log("DB Has ID And PW");
                return true;
            }
            return false;
        });
    }
    public PlayerLoginInfo AuthenticateUserNickname(string userNickName)
    {
        return AuthenticateUserCommon((currentPlayerInfo) =>
        {
            if (userNickName == currentPlayerInfo.NickName)
            {
                Debug.Log("DB Has Nickname");
                return true;
            }
            return false;
        });
    }

    public async Task<(bool,string)> WriteToGoogleSheet(string id, string password)
    {

        Spreadsheet sheet = Managers.DataManager.GetGoogleSheetData(GoogleDataBaseStruct, out SheetsService service, out string spreadsheetId,true);

        PlayerLoginInfo isIDInDatabase = AuthenticateUser(id);

        if(isIDInDatabase.Equals(default(PlayerLoginInfo)) == false)
        {
            return (false, "�̹� �ִ� ID �Դϴ�");
        }

        // �ۼ��� ������ �غ�
        List<IList<object>> values = new List<IList<object>>()
        {
            new List<object> { id, password} 
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
        try
        {
            AppendValuesResponse response = await appendRequest.ExecuteAsync();
        }
        catch(Exception ex)
        {
            Debug.Log($"{ex}������ �߻��߽��ϴ�");
            return (false, "DB�� ������ ������ �߻��߽��ϴ�.");
        }
        return (true, "ȸ�������� ���ϵ帳�ϴ�.");
    }


    public async Task<(bool,string)> WriteNickNameToGoogleSheet(PlayerLoginInfo playerInfo,string nickName)
    {
        Spreadsheet sheet = Managers.DataManager.GetGoogleSheetData(GoogleDataBaseStruct, out SheetsService service, out string spreadsheetId, true);

        PlayerLoginInfo isNickNameDatabase = AuthenticateUserNickname(nickName);

        if (isNickNameDatabase.Equals(default(PlayerLoginInfo)) == false)
        {
            return (false, "�̹� �ִ� �г��� �Դϴ�");
        }
        List<IList<object>> values = new List<IList<object>>()
        {
            new List<object> { nickName }
        };

        ValueRange valueRange = new ValueRange
        {
            Values = values
        };
        // ���� ����: ��Ʈ �̸��� ������ ����
        string writeRange = $"{USER_AUTHENTICATE_DATASHEET_NAME}!C{playerInfo.RowNumber+1}"; // ��: "A2:B2" Ư�� ��ġ
        SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest = service.Spreadsheets.Values.Update(valueRange, spreadsheetId, writeRange);
        updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        try
        {
            // ��û ����
            UpdateValuesResponse response = await updateRequest.ExecuteAsync();
        }
        catch (Exception ex)
        {
            return (false, "DB�� ���� �� ������ �߻��߽��ϴ�.");
        }

        return (true, "�г����� ���⼺��.");
    }

}