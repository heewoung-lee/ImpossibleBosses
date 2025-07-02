using NetWork.NGO;
using Zenject;

namespace ProjectContextInstaller
{
    public class RpcCallerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IFactory<NgoRPCCaller>>().To<NgoRPCCaller.NgoRPCCallerFactory>().AsSingle();
        }
    }
}
