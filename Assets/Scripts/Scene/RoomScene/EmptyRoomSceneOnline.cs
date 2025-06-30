using System.Threading.Tasks;

namespace Scene.RoomScene
{
    public class EmptyRoomSceneOnline : IRoomConnectOnline
    {
        public Task RoomSceneConnectOnlineStart()
        {
            return Task.CompletedTask;
        }
    }
}
