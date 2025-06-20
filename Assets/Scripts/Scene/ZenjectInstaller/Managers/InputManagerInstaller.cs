using GameManagers;
using Zenject;

namespace Scene.ZenjectInstaller
{
   public class InputManagerInstaller : MonoInstaller
   {
      public override void InstallBindings()
      {
         Container.BindInterfacesTo<InputManager>().AsSingle();
      }
   }
}
