using GameManagers;
using UnityEngine;
using Zenject;

namespace ProjectContextInstaller
{
    [DisallowMultipleComponent]
    public class SpreadSheetIDInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameDataSpreadSheet>().AsSingle();
        }
    }
}
