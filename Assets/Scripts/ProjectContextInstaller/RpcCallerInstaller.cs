using NetWork.NGO;
using Zenject;

namespace ProjectContextInstaller
{
    public class RpcCallerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {

            Container.BindFactory<NgoRPCCaller, NgoRPCCaller.Factory>()
                .FromComponentInNewPrefabResource("Prefabs/NGO/NgoRPCCaller");
        }
    }
}
