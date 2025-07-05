using System.Threading.Tasks;

namespace Scene.CommonInstaller
{
    public class EmptySceneOnline : ISceneConnectOnline
    {
        public Task SceneConnectOnlineStart()
        {
            return Task.CompletedTask;
        }
    }
}
