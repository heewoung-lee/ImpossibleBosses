using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace NetWork.NGO
{
    public abstract class NgoZenjectFactory<T> : IFactory<T> where T : NetworkBehaviour
    {
        protected DiContainer _container;
        protected GameObject _ngo;
        public T Create()
        {
            GameObject createdNgo = UnityEngine.Object.Instantiate(_ngo);
            _container.InjectGameObject(createdNgo);
            
            return createdNgo.GetComponent<T>();
        }
    }
}
