using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using Zenject;

namespace NetWork.NGO
{
    public class NgoZenjectHandler: INetworkPrefabInstanceHandler
    {
        private readonly DiContainer _diContainer;
        private readonly NetworkObject _networkObj;
        
        public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
        {
            _diContainer.InjectGameObject(_networkObj.gameObject);
            return _networkObj;
        }

        public void Destroy(NetworkObject networkObject)
        {
            Object.Destroy(networkObject.gameObject);
                //TODO: 꼭 바꿔야함 현재 오브젝트들의 인터페이스가 없기에 Object.Delete를 고정했지만.
                //Delete 인터페이스를 만들어서 풀에 들어가야하는거, 없애하는거 등 인터페이스로 삭제를 유도해야함
        }
    }
}
