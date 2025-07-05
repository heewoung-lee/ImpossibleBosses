using GameManagers;
using GameManagers.Interface.ResourcesManager;
using Module.UI_Module;
using Scene.BattleScene;
using Scene.CommonInstaller;
using Scene.RoomScene;
using UnityEngine;
using Zenject;

namespace Scene.GamePlayScene.Installer
{
    public class GamePlaySceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            PlayScene playScene = FindAnyObjectByType<PlayScene>();
            BindTestMode();
            
            void BindTestMode()
            {
                Container.Bind<ISceneStarter>().To<PlaySceneStarter>().AsSingle();
                Container.Bind<ISceneMover>().To<BattleSceneMover>().AsSingle();
                if (SceneManagerEx.IsNormalBoot == true || playScene.GetTestMode() == TestMode.Local)
                {
                    Container.Bind<ISceneConnectOnline>().To<EmptySceneOnline>().AsSingle();
                    Container.Bind<ISceneSpawnBehaviour>().To<UnitNetGamePlayScene>().AsSingle();
                }
                else
                {
                    if (playScene.GetMultiTestMode() == MultiMode.Solo)
                    {
                        Container.Bind<ISceneConnectOnline>().To<SceneConnectOnlineSolo>().AsSingle();
                        Container.Bind<ISceneSpawnBehaviour>().To<MockGamePlayScene>().AsSingle();
                    }
                    else if (playScene.GetMultiTestMode() == MultiMode.Multi)
                    {
                        Container.Bind<ISceneConnectOnline>().To<SceneConnectOnlineMulti>().AsSingle();
                        Container.Bind<ISceneSpawnBehaviour>().To<MockGamePlayScene>().AsSingle();
                    }
                }
                
            }
        }
      
    }
}
