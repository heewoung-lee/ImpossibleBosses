using GameManagers;
using NetWork.BaseNGO;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace NetWork.NGO.InitializeNGO
{
    public class NgoVFXInitalize : NgoInitailizeBase
    {
        public class VFXRootNgoFactory : NgoZenjectFactory<NgoVFXInitalize>
        {
            public VFXRootNgoFactory(DiContainer container, GameObject ngo)
            {
                _container = container;
                _ngo = ngo;
            }
        }
        private NetworkObject _vfxRootNgo;

        public override NetworkObject ParticleNgo => _vfxRootNgo;

        public override void SetInitalze(NetworkObject obj)
        {
            Managers.VFXManager.Set_VFX_Root_NGO(obj);
        }


        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Managers.VFXManager.Set_VFX_Root_NGO(gameObject.GetComponent<NetworkObject>());
        }
    }
}
