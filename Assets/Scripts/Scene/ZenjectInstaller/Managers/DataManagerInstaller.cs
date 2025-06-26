using GameManagers;
using GameManagers.Interface.DataManager;
using Zenject;

namespace Scene.ZenjectInstaller.Managers
{
    public class DataManagerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<DataManager>().AsSingle();
        }
    }
}
