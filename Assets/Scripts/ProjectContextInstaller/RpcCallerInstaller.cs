using NetWork.NGO;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace ProjectContextInstaller
{
    public class RpcCallerInstaller : MonoInstaller
    {
        private GameObject _ngo;
        public override void InstallBindings()
        {
            _ngo = Resources.Load<GameObject>("Prefabs/NGO/NgoRPCCaller");
            Container.Bind<IFactory<NgoRPCCaller>>().To<NgoRPCCaller.NgoRPCCallerFactory>().AsSingle().WithArguments(_ngo);
        }
        
        public override void Start()
        {
            base.Start();
            Debug.Log("Starting RPCCallerInstaller");

            if (NetworkManager.Singleton == null)
                return;
            
            NgoZenjectHandler handler = new NgoZenjectHandler(Container,_ngo);
            NetworkManager.Singleton.PrefabHandler.AddHandler(_ngo, handler);
            Debug.Log("등록");
        }
    }
}
