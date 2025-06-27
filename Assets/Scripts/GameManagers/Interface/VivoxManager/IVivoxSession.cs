using System;
using System.Threading.Tasks;

namespace GameManagers.Interface.VivoxManager
{
    public interface IVivoxSession
    {
        public Task JoinChannelAsync(string chanelID);
        public Task LogoutOfVivoxAsync();
        public bool CheckDoneLoginProcess { get; }
        public event Action VivoxDoneLoginEvent;
    }
}
