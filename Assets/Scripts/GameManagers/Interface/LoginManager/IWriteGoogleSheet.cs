using System.Threading.Tasks;

namespace GameManagers.Interface.LoginManager
{
    public interface IWriteGoogleSheet
    {
        public Task<(bool, string)> WriteToGoogleSheet(string id, string password);
        public Task<(bool, string)> WriteNickNameToGoogleSheet(PlayerLoginInfo playerInfo, string nickName);
    }
}
