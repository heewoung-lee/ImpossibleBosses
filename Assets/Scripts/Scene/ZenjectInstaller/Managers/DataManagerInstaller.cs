using GameManagers;
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
