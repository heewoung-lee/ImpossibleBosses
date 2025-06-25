using GameManagers;
using Zenject;

namespace ProjectContextInstaller
{
    public class GameManagerExInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameManagerEx>().AsSingle();
        }
    }
}
