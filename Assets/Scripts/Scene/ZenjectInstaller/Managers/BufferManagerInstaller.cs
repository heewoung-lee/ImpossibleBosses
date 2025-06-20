using GameManagers;
using Zenject;

namespace Scene.ZenjectInstaller.Managers
{
    public class BufferManagerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<BufferManager>().AsSingle();            
        }
    }
}
