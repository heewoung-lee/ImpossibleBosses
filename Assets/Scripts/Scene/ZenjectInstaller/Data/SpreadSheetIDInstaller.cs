using GameManagers;
using UnityEngine;
using Zenject;

namespace Scene.ZenjectInstaller.Data
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
