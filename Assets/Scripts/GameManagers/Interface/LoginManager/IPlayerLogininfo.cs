namespace GameManagers.Interface.LoginManager
{
    public interface IPlayerLogininfo
    {
        public PlayerLoginInfo GetCurrentPlayerInfo();

        public void SetPlayerInfo(PlayerLoginInfo playerInfo);
        public PlayerLoginInfo FindAuthenticateUser(string userID, string userPW);
    }
}
