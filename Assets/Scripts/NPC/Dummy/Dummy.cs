using NetWork.NGO;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace NPC.Dummy
{
    public class Dummy : NetworkBehaviour
    {
        public class DummyFactory : NgoZenjectFactory<Dummy>
        {
            public DummyFactory(DiContainer container,GameObject ngo)
            {
            _container = container;
            _ngo = ngo;
            }
        }
    }
}
