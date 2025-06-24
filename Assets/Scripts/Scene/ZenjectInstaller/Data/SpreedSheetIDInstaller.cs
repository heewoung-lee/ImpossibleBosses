using UnityEngine;
using Zenject;

namespace Scene.ZenjectInstaller.Data
{
    public class SpreedSheetIDInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<SpreedSheetID>().AsSingle();
        }
    }
}
