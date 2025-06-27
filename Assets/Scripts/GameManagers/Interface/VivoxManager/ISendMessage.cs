using System.Threading.Tasks;

namespace GameManagers.Interface.VivoxManager
{
    public interface ISendMessage
    {
        public Task SendSystemMessageAsync(string systemMessage);
        public Task SendMessageAsync(string message);
    }
}
