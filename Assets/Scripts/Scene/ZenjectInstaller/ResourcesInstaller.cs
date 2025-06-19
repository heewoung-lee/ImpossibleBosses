using GameManagers;
using Zenject;

namespace Scene.ZenjectInstaller
{
   public class ResourcesInstaller : MonoInstaller
   {
      public override void InstallBindings()
      {
         Container.Bind<ResourceManager>().AsSingle().NonLazy();
      }
   }
}
