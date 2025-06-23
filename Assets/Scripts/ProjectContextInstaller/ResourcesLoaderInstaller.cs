using GameManagers;
using GameManagers.Interface.Resources_Interface;
using Zenject;

namespace ProjectContextInstaller
{
    public class ResourcesLoaderInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ResourceManager>().AsSingle();
        }
    }
}
