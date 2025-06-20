using GameManagers;
using Zenject;

namespace Scene.ZenjectInstaller.Managers
{
   public class ResourcesInstaller : MonoInstaller
   {
      public override void InstallBindings()
      {
         Container.BindInterfacesTo<ResourceManager>().AsSingle().NonLazy();
      }
   }
}
