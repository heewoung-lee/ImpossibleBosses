using GameManagers;
using GameManagers.Interface.DataManager;
using UnityEngine;
using Zenject;

namespace ProjectContextInstaller
{
    [DisallowMultipleComponent]
    public class AllDataInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IAllData>().To<AllData>().AsSingle().NonLazy();
        }
    }
}
