using GameManagers;
using Zenject;

namespace Scene.ZenjectInstaller.Managers
{
   public class ItamDataManagerInstaller : MonoInstaller
   {
      public override void InstallBindings()
      {
         Container.Bind<ItemDataManager>().AsSingle();
      }
   }
}
