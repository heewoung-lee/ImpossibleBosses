using GameManagers;
using Scene.CommonInstaller;
using Zenject;

namespace Scene.RoomScene.Installer
{
   public class RoomSceneInstaller : MonoInstaller
   {
      public override void InstallBindings()
      {
         RoomPlayScene roomscene = FindAnyObjectByType<RoomPlayScene>();
         
         Container.Bind<ISceneStarter>().To<RoomSceneStarter>().AsSingle();
         if (SceneManagerEx.IsNormalBoot == true || roomscene.GetTestMode() == TestMode.Local)
         {
            Container.Bind<ISceneConnectOnline>().To<EmptySceneOnline>().AsSingle();
         }
         else
         {
            if (roomscene.GetMultiTestMode() == MultiMode.Solo)
            {
               Container.Bind<ISceneConnectOnline>().To<SceneConnectOnlineSolo>().AsSingle();
            }
            else if (roomscene.GetMultiTestMode() == MultiMode.Multi)
            {
               Container.Bind<ISceneConnectOnline>().To<SceneConnectOnlineMulti>().AsSingle();
            }
         }
      }
   }
}
