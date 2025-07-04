using System.Threading.Tasks;

namespace Scene.CommonInstaller
{
    public enum PlayersTag
    {
        Player1,
        Player2,
        Player3,
        Player4,
        None
    }
    
    public interface ISceneConnectOnline
    { 
        public Task SceneConnectOnlineStart();
    }
}
