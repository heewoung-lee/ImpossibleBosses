using System.Threading.Tasks;

namespace Scene.RoomScene
{
    public enum PlayersTag
    {
        Player1,
        Player2,
        Player3,
        Player4,
        None
    }
    
    public interface IRoomConnectOnline
    { 
        public Task RoomSceneConnectOnlineStart();
    }
}
