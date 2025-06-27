using GameManagers;
using Zenject;

namespace ProjectContextInstaller
{
    public class SceneManagerExInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<SceneManagerEx>().AsSingle();
        }
    }
}
