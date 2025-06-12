using GameManagers;
using NetWork.BaseNGO;
using Unity.Netcode;

namespace NetWork.NGO.InitializeNGO
{
    public class NgoVFXInitalize : NgoInitailizeBase
    {
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
