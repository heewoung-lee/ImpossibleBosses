using GameManagers;
using Zenject;

namespace Scene.ZenjectInstaller.Managers
{
   public class UIManagerInstaller : MonoInstaller
   {
      public override void InstallBindings()
      {
         Container.BindInterfacesTo<UIManager>().AsSingle();
      }
   }
}
