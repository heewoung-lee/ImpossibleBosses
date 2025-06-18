using GameManagers;
using Scene.GamePlayScene;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class PlaySceneInstaller : MonoInstaller
{
   private PlayScene _scene;
   public override void InstallBindings()
   {
      _scene = GetComponent<PlayScene>();

      Container.Bind<ISceneSpawnBehaviour>()
         .FromInstance(SelectBehaviour())
         .AsSingle();

   }

   private ISceneSpawnBehaviour SelectBehaviour()
   {
      _scene.AddComponent<UIGamePlaySceneModule>();
      
      if (Managers.SceneManagerEx.IsNormalBoot)
      {
         return new UnitNetGamePlayScene();
      }

      
      if (_scene.testMode == PlayScene.TestMode.Local)
         return new UnitLocalGamePlayScene();


      if (_scene.testMode == PlayScene.TestMode.Multi)
      {
         if (_scene.multiMode == PlayScene.MultiMode.Solo)
         {
            return new MockUnitGamePlayScene(_scene.playerableCharacter,true);
         }
      }
      return new MockUnitGamePlayScene(_scene.playerableCharacter,false);
   }
}
