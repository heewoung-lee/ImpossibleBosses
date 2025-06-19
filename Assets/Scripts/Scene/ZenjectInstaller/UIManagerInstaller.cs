using GameManagers;
using GameManagers.Interface;
using Zenject;

namespace Scene.ZenjectInstaller
{
   public class UIManagerInstaller : MonoInstaller
   {
      public override void InstallBindings()
      {
         Container.BindInterfacesTo<UIManager>().AsSingle();
      }
   }
}
