using GameManagers;
using GameManagers.Interface.Resources_Interface;
using UnityEngine;
using Zenject;

namespace ProjectContextInstaller
{
    [DisallowMultipleComponent]
    public class ResourcesLoaderInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ResourceManager>().AsSingle().NonLazy();
        }
    }
}
