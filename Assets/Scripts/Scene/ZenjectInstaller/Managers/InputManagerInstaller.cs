using GameManagers;
using Zenject;

namespace Scene.ZenjectInstaller.Managers
{
   public class InputManagerInstaller : MonoInstaller
   {
      public override void InstallBindings()
      {
         Container.BindInterfacesTo<InputManager>().AsSingle();
      }
   }
}
