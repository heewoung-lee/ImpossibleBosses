using Zenject;
using ZenjectTool;

namespace Scene.CommonInstaller
{
    public class NgoInjectorInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<NgoInjector>().AsSingle();
        }
    }
}
