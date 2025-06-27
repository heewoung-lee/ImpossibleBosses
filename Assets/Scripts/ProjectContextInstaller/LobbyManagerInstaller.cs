using GameManagers;
using UnityEngine;
using Zenject;

namespace ProjectContextInstaller
{
    public class LobbyManagerInstaller : MonoInstaller
    {
        //TODO: 인터페이스 분리 필요
        public override void InstallBindings()
        {
            Container.Bind<LobbyManager>().AsSingle();
        }
    }
}