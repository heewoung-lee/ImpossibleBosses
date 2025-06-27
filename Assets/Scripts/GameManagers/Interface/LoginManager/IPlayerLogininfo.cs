namespace GameManagers.Interface.LoginManager
{
    public interface IPlayerLogininfo
    {
        public string PlayerNickName { get;}
        public PlayerLoginInfo FindAuthenticateUser(string userID, string userPW);
    }
}
