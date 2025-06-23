using System;
using GameManagers;
using GameManagers.Interface.Resources_Interface;
using Unity.VisualScripting;
using Zenject;

namespace Scene.ZenjectInstaller.Managers
{
    public class CachingObjectDictInstaller : MonoInstaller
    {
        [Inject] IResourcesLoader _resourcesLoader;
        
        public override void InstallBindings()
        {
            Container.Bind<CachingObjectDictManager>().AsSingle();
        }

        public override void Start()
        {
            base.Start();
            var cachingObjectDictManager = Container.Resolve<CachingObjectDictManager>();
            _resourcesLoader.ResisterCacheManager(cachingObjectDictManager);
        }
    }   
}
