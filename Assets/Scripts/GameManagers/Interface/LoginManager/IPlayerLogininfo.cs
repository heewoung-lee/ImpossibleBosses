namespace GameManagers.Interface.LoginManager
{
    public interface IPlayerLogininfo
    {
        public PlayerLoginInfo CurrentPlayerInfo { get; }
        public PlayerLoginInfo FindAuthenticateUser(string userID, string userPW);
    }
}
