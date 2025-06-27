using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameManagers.Interface.DataManager;
using GameManagers.Interface.LoginManager;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using UnityEngine;
using Zenject;

namespace GameManagers
{
    public struct PlayerLoginInfo : IEquatable<PlayerLoginInfo>
    {
        public string ID;
        public string Password;
        public string NickName;
        public int RowNumber;

        public bool Equals(PlayerLoginInfo other)
        {
            return ID == other.ID && Password == other.Password;
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerLoginInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, Password);
        }
    }

    public class LogInManager:IPlayerLogininfo,IWriteGoogleSheet,IPlayerIngameLogininfo
    {
        [Inject] IGoogleDataBaseStruct _dataManager;
        [Inject] ILoginDataSpreadSheet _loginDataSpreadSheet;
        enum SheetIndex
        {
            ID,
            PW,
            NickName,
        }
        
        private string LoginDataSpreadsheetID => _loginDataSpreadSheet.LoginDataSpreadsheetID;
        private string UserAuthenticateDatasheetName => _loginDataSpreadSheet.UserAuthenticateDatasheetName;
        private PlayerIngameLoginInfo _playerIngameLoginInfo;
        private PlayerLoginInfo _currentPlayerInfo;
        private GoogleDataBaseStruct _googleDataBaseStruct;
        GoogleDataBaseStruct GoogleUserDataSheet
        {
            get
            {
                if(_googleDataBaseStruct.Equals(default(GoogleDataBaseStruct)))
                {
                    _googleDataBaseStruct = _dataManager.DataBaseStruct;
                    _googleDataBaseStruct.SpreedSheetID = LoginDataSpreadsheetID;
                }
                return _googleDataBaseStruct;
            }
        }
        private PlayerLoginInfo AuthenticateUserCommon(Func<PlayerLoginInfo, bool> action)
        {
            //구글 스프레드 시트에 접근해서 맞는 아이디와 패스워드를 확인한후
            //있으면 로비창으로 씬전환
            //없으면 오류메세지 출력
            Spreadsheet spreadsheet = _dataManager.GetGoogleSpreadsheet(GoogleUserDataSheet, out SheetsService service, out string spreadsheetId);
            Sheet userAthenticateData = null;
            bool ischeckSamePlayerLoginfo = false;
            foreach (Sheet sheet in spreadsheet.Sheets)
            {
                if (sheet.Properties.Title == UserAuthenticateDatasheetName)
                {
                    userAthenticateData = sheet;
                    break;
                }
            }

            if (userAthenticateData == null)
            {
                Debug.LogError($"Not Found {UserAuthenticateDatasheetName} Sheet");
                return default;
            }

            string range = $"{userAthenticateData.Properties.Title}!A1:Z"; // 필요한 범위 지정 전부 다 읽겠다.
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

                    _currentPlayerInfo = new PlayerLoginInfo()
                    {
                        ID = idData,
                        Password = pwData,
                        NickName = nickNameData,
                        RowNumber = rowIndex,
                    };
                }
                ischeckSamePlayerLoginfo = action.Invoke(_currentPlayerInfo);

                if (ischeckSamePlayerLoginfo == false)
                    continue;
                else
                    return _currentPlayerInfo;
            }
            return default;
        }

        public string PlayerNickName => _currentPlayerInfo.NickName;

        public PlayerLoginInfo FindAuthenticateUser(string userID, string userPW)
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
        private PlayerLoginInfo FindUserById(string userID)
        {
            return AuthenticateUserCommon((currentPlayerInfo) =>
            {
                if (userID == currentPlayerInfo.ID)
                {
                    Debug.Log("DB Has ID");
                    return true;
                }
                return false;
            });
        }
        private PlayerLoginInfo FindUserByNickName(string userNickName)
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

            Spreadsheet sheet = _dataManager.GetGoogleSpreadsheet(GoogleUserDataSheet, out SheetsService service, out string spreadsheetId,true);

            PlayerLoginInfo isIDInDatabase = FindUserById(id);

            if(isIDInDatabase.Equals(default(PlayerLoginInfo)) == false)
            {
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
            string range = $"{UserAuthenticateDatasheetName}!A1:Z"; // 시트 이름과 범위 (A열~C열)

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
        public async Task<(bool,string)> WriteNickNameToGoogleSheet(PlayerLoginInfo playerInfo,string nickName)
        {
            Spreadsheet sheet = _dataManager.GetGoogleSpreadsheet(GoogleUserDataSheet, out SheetsService service, out string spreadsheetId, true);

            PlayerLoginInfo isNickNameDatabase = FindUserByNickName(nickName);

            if (isNickNameDatabase.Equals(default(PlayerLoginInfo)) == false)
            {
                return (false, "이미 있는 닉네임 입니다");
            }
            List<IList<object>> values = new List<IList<object>>()
            {
                new List<object> { nickName }
            };

            ValueRange valueRange = new ValueRange
            {
                Values = values
            };
            // 범위 설정: 시트 이름과 범위를 지정
            string writeRange = $"{UserAuthenticateDatasheetName}!C{playerInfo.RowNumber+1}"; // 예: "A2:B2" 특정 위치
            SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest = service.Spreadsheets.Values.Update(valueRange, spreadsheetId, writeRange);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            try
            {
                // 요청 실행
                UpdateValuesResponse response = await updateRequest.ExecuteAsync();
            }
            catch (Exception ex)
            {
                return (false, $"DB를 쓰는 중 오류가 발생했습니다.에러코드: {ex}");
            }

            return (true, "닉네임을 짓기성공.");
        }
        public PlayerIngameLoginInfo GetPlayerIngameLoginInfo()
        {
            return _playerIngameLoginInfo;
        }
        public void SetPlayerIngameLoginInfo(PlayerIngameLoginInfo playerIngameLoginInfo)
        {
            _playerIngameLoginInfo = playerIngameLoginInfo;
        }
    }
}