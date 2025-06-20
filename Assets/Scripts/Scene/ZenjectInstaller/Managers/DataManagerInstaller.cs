using GameManagers;
using Zenject;

namespace Scene.ZenjectInstaller.Managers
{
    public class DataManagerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<DataManager>().AsSingle();
        }
    }
}
