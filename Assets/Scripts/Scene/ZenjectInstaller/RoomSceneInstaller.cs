using System;
using GameManagers;
using Scene.RoomScene;
using Zenject;

namespace Scene.ZenjectInstaller
{
   public class RoomSceneInstaller : MonoInstaller
   {
      [Inject] SceneManagerEx _sceneManagerEx;
      public override void InstallBindings()
      {
         RoomPlayScene roomscene = FindAnyObjectByType<RoomPlayScene>();
         
         Container.Bind<IRoomSceneStarter>().To<RoomSceneStarter>().AsSingle();
         if (_sceneManagerEx.IsNormalBoot == true || roomscene.GetTestMode() == TestMode.Local)
         {
            Container.Bind<IRoomConnectOnline>().To<EmptyRoomSceneOnline>().AsSingle();
         }
         else
         {
            if (roomscene.GetMultiTestMode() == MultiMode.Solo)
            {
               Container.Bind<IRoomConnectOnline>().To<RoomSceneConnectOnlineSolo>().AsSingle();
            }
            else if (roomscene.GetMultiTestMode() == MultiMode.Multi)
            {
               Container.Bind<IRoomConnectOnline>().To<RooMSceneConnectOnlineMulti>().AsSingle();
            }
         }
      }
   }
}
