using System.Collections.Generic;
using NetWork.NGO;
using NetWork.NGO.InitializeNGO;
using NetWork.NGO.UI;
using NPC.Dummy;
using Scene.CommonInstaller;
using UnityEngine;
using Zenject;

namespace Scene.GamePlayScene.Installer
{
    public class GamePlaySceneUseObjectInstaller : BindNgoHandlerInstaller
    {
        private List<GameObject> _sceneObjects;

        public override void InstallBindings()
        {
            _sceneObjects = new List<GameObject>(10);
            
            
            GameObject ngoGamePlaySceneSpawn = Resources.Load<GameObject>("Prefabs/NGO/NgoGamePlaySceneSpawn");
            Container.Bind<IFactory<NgoGamePlaySceneSpawn>>().To<NgoGamePlaySceneSpawn.NgoGamePlaySceneSpawnFactory>()
                .AsSingle().WithArguments(ngoGamePlaySceneSpawn);
            
            _sceneObjects.Add(ngoGamePlaySceneSpawn);
            
            
            GameObject ngoVFXRoot = Resources.Load<GameObject>("Prefabs/NGO/VFXRootNGO");
            Container.Bind<IFactory<NgoVFXInitalize>>().To<NgoVFXInitalize.VFXRootNgoFactory>()
                .AsSingle().WithArguments(ngoVFXRoot);
            
            _sceneObjects.Add(ngoVFXRoot);

            
            
            GameObject dummy = Resources.Load<GameObject>("Prefabs/NPC/DamageTestDummy");
            Container.Bind<IFactory<Dummy>>().To<Dummy.DummyFactory>()
                .AsSingle().WithArguments(dummy);
            
            _sceneObjects.Add(dummy);
            
        }

        public override void Start()
        {
            base.Start();
            InputBindNgoHandler(_sceneObjects);
        }
    }
}
