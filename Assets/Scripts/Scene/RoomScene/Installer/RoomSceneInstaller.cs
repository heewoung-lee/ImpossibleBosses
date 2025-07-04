using GameManagers;
using Scene.CommonInstaller;
using Zenject;

namespace Scene.RoomScene.Installer
{
   public class RoomSceneInstaller : MonoInstaller
   {
      [Inject] SceneManagerEx _sceneManagerEx;
      public override void InstallBindings()
      {
         RoomPlayScene roomscene = FindAnyObjectByType<RoomPlayScene>();
         
         Container.Bind<ISceneStarter>().To<RoomSceneStarter>().AsSingle();
         if (_sceneManagerEx.IsNormalBoot == true || roomscene.GetTestMode() == TestMode.Local)
         {
            Container.Bind<ISceneConnectOnline>().To<EmptyRoomSceneOnline>().AsSingle();
         }
         else
         {
            if (roomscene.GetMultiTestMode() == MultiMode.Solo)
            {
               Container.Bind<ISceneConnectOnline>().To<RoomSceneConnectOnlineSolo>().AsSingle();
            }
            else if (roomscene.GetMultiTestMode() == MultiMode.Multi)
            {
               Container.Bind<ISceneConnectOnline>().To<RoomSceneConnectOnlineMulti>().AsSingle();
            }
         }
      }
   }
}
