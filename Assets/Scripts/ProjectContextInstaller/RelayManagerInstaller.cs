using GameManagers;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace ProjectContextInstaller
{
    public class RelayManagerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<RelayManager>().AsSingle();
        }
    }
}
