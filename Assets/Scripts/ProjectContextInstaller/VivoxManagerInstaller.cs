using GameManagers;
using UnityEngine;
using Zenject;

namespace ProjectContextInstaller
{
    public class VivoxManagerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<VivoxManager>().AsSingle();
        }
    }
}
