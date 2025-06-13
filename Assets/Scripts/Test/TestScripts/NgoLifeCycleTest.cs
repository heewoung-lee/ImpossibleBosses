using Unity.Netcode;
using UnityEngine;

namespace Test.TestScripts
{
    public class NgoLifeCycleTest : NetworkBehaviour
    {
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            Debug.Log($"{gameObject.name} NGODeSpawn: {System.Environment.StackTrace}");
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Debug.Log($"{gameObject.name}has a ParentTr when NGO Spawn {(gameObject.transform.parent is null ? "No parent object" : transform.parent.name)} stackTrace:{System.Environment.StackTrace}");

        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            Debug.Log($"{gameObject.name} OnDestroy: {System.Environment.StackTrace}");
        }
        private void OnTransformParentChanged()
        {
            Debug.Log($"{gameObject.name}OnParentTrChanged {(gameObject.transform.parent is null ? "No parent object" : transform.parent.name)}");
        }

    }
}
