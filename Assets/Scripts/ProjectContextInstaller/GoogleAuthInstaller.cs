using Data;
using GameManagers;
using UnityEngine;
using Zenject;

namespace ProjectContextInstaller
{
    [DisallowMultipleComponent]
    public class GoogleAuthInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GoogleAuthLogin>().AsSingle().NonLazy();
        }
    }
}
