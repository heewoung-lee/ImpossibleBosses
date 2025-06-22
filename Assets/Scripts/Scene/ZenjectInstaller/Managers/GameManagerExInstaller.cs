using GameManagers;
using Zenject;

namespace Scene.ZenjectInstaller.Managers
{
   public class GameManagerExInstaller : MonoInstaller
   {
      public override void InstallBindings()
      {
         Container.Bind<GameManagerEx>().AsSingle();
      }
   }
}
