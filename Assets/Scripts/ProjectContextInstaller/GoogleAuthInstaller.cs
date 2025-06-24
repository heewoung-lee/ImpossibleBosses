using Data;
using GameManagers;
using Zenject;

namespace ProjectContextInstaller
{
    public class GoogleAuthInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GoogleAuthLogin>().AsSingle().NonLazy();
        }
    }
}
