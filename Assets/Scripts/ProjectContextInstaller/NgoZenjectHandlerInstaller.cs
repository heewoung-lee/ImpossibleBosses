using NetWork.NGO;
using UnityEngine;
using Zenject;

namespace ProjectContextInstaller
{
    public class NgoZenjectHandlerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindFactory<GameObject, NgoZenjectHandler,NgoZenjectHandler.NgoZenjectHandlerFactory>();
        }
    }
}
