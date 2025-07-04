using System.Collections.Generic;
using NetWork.NGO;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using ZenjectTool;

namespace ProjectContextInstaller
{
    public class RpcCallerInstaller : BindNgoHandlerInstaller
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
            InputBindNgoHandler(new List<GameObject>(){_ngo});
        }
    }
}
