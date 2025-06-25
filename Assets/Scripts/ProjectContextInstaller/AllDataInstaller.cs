using GameManagers;
using GameManagers.Interface.DataManager;
using UnityEngine;
using Zenject;

namespace ProjectContextInstaller
{
    public class AllDataInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IAllData>().To<AllData>().AsSingle();
        }
    }
}
