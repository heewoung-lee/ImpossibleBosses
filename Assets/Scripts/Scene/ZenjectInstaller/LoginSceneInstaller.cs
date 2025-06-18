using GameManagers;
using Zenject;

namespace Scene.ZenjectInstaller
{
   public class LoginSceneInstaller : MonoInstaller
   {
      public override void InstallBindings()
      {
         Container.Bind<UIManager>().FromInstance(new UIManager()).AsSingle();
      }
   }
}
