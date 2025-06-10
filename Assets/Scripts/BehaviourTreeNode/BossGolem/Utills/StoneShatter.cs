using UnityEngine;

namespace BehaviourTreeNode.BossGolem.Utills
{
    public class StoneShatter : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            // 예: 플레이어나 바닥과 충돌 시 처리
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Shatter();
            }
        }
        void Shatter()
        {
            GetComponent<Collider>().enabled = false;
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out Rigidbody childRb))
                {
                    childRb.isKinematic = false;
                    childRb.useGravity = true;
                }
                childRb.AddExplosionForce(200f, transform.position, 5f);
            }
        }

    }
}
