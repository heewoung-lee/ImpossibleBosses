using GameManagers;
using Unity.VisualScripting;
using Zenject;

namespace Scene.GamePlayScene.Installer
{
   public class PlaySceneInstaller : MonoInstaller
   {
      [Inject] SceneManagerEx _sceneManagerEx;
      private PlayScene _scene;
      public override void InstallBindings()
      {
         _scene = _sceneManagerEx.GetCurrentScene as PlayScene;

         Container.Bind<ISceneSpawnBehaviour>()
            .FromInstance(SelectBehaviour())
            .AsSingle();

      }

      private ISceneSpawnBehaviour SelectBehaviour()
      {
         _scene.AddComponent<UIGamePlaySceneModule>();
      
         if (_sceneManagerEx.IsNormalBoot)
         {
            return new UnitNetGamePlayScene();
         }

      
         if (_scene.GetTestMode() == TestMode.Local)
            return new UnitLocalGamePlayScene();


         if (_scene.GetTestMode() == TestMode.Multi)
         {
            if (_scene.GetMultiTestMode() == MultiMode.Solo)
            {
               return new MockUnitGamePlayScene(_scene.playerableCharacter,true);
            }
         }
         return new MockUnitGamePlayScene(_scene.playerableCharacter,false);
      }
   }
}
