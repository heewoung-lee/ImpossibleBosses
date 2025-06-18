using GameManagers;
using Zenject;

namespace Scene.ZenjectInstaller
{
   public class RoomSceneInstaller : MonoInstaller
   {
      public override void InstallBindings()
      {
         Container.Bind<UIManager>().FromInstance(new UIManager()).AsSingle();
      }
   }
}
