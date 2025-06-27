namespace GameManagers.Interface.LoginManager
{
    public struct PlayerIngameLoginInfo
    {
        private readonly string _nickname;
        private readonly string _id;

        public string PlayerNickName => _nickname;
        public string Id => _id;

        public PlayerIngameLoginInfo(string playerNickname, string playerId)
        {
            _nickname = playerNickname; // 모든 필드를 초기화
            _id = playerId;
        }
    }
    public interface IPlayerIngameLogininfo
    {
        public PlayerIngameLoginInfo GetPlayerIngameLoginInfo();
        public void SetPlayerIngameLoginInfo(PlayerIngameLoginInfo playerIngameLoginInfo);
    }
}