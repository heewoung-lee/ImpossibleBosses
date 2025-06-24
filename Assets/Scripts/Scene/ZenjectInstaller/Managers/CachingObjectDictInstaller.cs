using System;
using GameManagers;
using GameManagers.Interface.Resources_Interface;
using Unity.VisualScripting;
using Zenject;

namespace Scene.ZenjectInstaller.Managers
{
    public class CachingObjectDictInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ICachingObjectDict>().To<CachingObjectDictManager>()
                .AsSingle().NonLazy();
        }
    }   
}
