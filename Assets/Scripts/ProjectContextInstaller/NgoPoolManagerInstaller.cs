using GameManagers;
using UnityEngine;
using Zenject;

namespace ProjectContextInstaller
{
    public class NgoPoolManagerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<NgoPoolManager>().AsSingle();
        }
    }
}
