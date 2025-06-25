using GameManagers.Interface.DataManager;

namespace GameManagers
{
    public class GameDataSpreadSheet : IGameDataSpreadSheet,ILoginDataSpreadSheet
    {
        private const string _gameDataSpreadsheetID = "1DV5kuhzjcNs3id8deI8Q3xFgbOWTa_pr76uSD0gNpGg";
        private const string _loginDataSpreadsheetID = "1SKhi41z1KRfHI6KwhQ2iM3mSjgLZKXw7_VopoIEZYNQ";
        private const string _userAuthenticateDatasheetName  = "UserAuthenticateData";
        
        public string GameDataSpreadsheetID => _gameDataSpreadsheetID;
        public string LoginDataSpreadsheetID => _loginDataSpreadsheetID;
        public string UserAuthenticateDatasheetName => _userAuthenticateDatasheetName;

    }
}