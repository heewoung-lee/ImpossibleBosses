using GameManagers;
using Zenject;

namespace ProjectContextInstaller
{
   public class ItamDataManagerInstaller : MonoInstaller
   {
      public override void InstallBindings()
      {
         Container.BindInterfacesTo<ItemDataManager>().AsSingle().NonLazy();
      }
   }
}
