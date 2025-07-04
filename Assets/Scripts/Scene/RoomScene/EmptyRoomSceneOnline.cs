using System.Threading.Tasks;
using Scene.CommonInstaller;

namespace Scene.RoomScene
{
    public class EmptyRoomSceneOnline : ISceneConnectOnline
    {
        public Task SceneConnectOnlineStart()
        {
            return Task.CompletedTask;
        }
    }
}
