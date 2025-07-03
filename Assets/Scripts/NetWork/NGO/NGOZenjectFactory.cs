using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace NetWork.NGO
{
    public abstract class NgoZenjectFactory<T> : IFactory<T> where T : NetworkBehaviour
    {
        protected DiContainer _container;
        protected abstract string Path { get; }
        
        public T Create()
        {
            GameObject prefab = Resources.Load<GameObject>(Path);
            GameObject createdNgo = UnityEngine.Object.Instantiate(prefab);
            _container.InjectGameObject(createdNgo);
            
            return createdNgo.GetComponent<T>();
        }
    }
}
