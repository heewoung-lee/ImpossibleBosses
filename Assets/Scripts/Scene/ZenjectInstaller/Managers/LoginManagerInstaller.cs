using GameManagers;
using UnityEngine;
using Zenject;

namespace Scene.ZenjectInstaller.Managers
{

    public class LoginManagerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<LogInManager>().AsSingle();
        }
    }
}
