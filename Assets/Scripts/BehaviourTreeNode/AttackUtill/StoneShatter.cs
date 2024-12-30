using UnityEngine;

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
        // 2) 자식 파편들 각각에 대해 물리 활성화
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out Rigidbody childRb))
            {
                // isKinematic을 false로 바꿔 물리 시뮬레이션 참여
                childRb.isKinematic = false;
                childRb.useGravity = true;
            }
            // 조금 더 ‘튀는’ 효과를 주고 싶다면 AddExplosionForce 등의 임의의 힘 추가
            childRb.AddExplosionForce(200f, transform.position, 5f);
        }
    }

}
