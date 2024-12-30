using UnityEngine;

public class StoneShatter : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        // 예: 플레이어나 바닥과 충돌 시 처리
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Ground"))
        {
            Shatter();
        }
    }


    void Shatter()
    {
        // 1) 부모 자신의 MeshRenderer/Collider 끄기 (또는 파괴)
        //    - 시각적인 돌 오브젝트가 사라지고, 이제 파편들이 보이도록 할 수 있음.
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        // 필요하다면 Rigidbody도 끄거나 Destroy해버릴 수 있음
        // Destroy(GetComponent<Rigidbody>());

        // 2) 자식 파편들 각각에 대해 물리 활성화
        foreach (Transform child in transform)
        {
            Rigidbody childRb = child.GetComponent<Rigidbody>();
            Collider childCol = child.GetComponent<Collider>();
            if (childRb != null)
            {
                // isKinematic을 false로 바꿔 물리 시뮬레이션 참여
                childRb.isKinematic = false;
            }
            if (childCol != null)
            {
                childCol.enabled = true;
            }

            // 부모에서 분리(자유롭게 움직이도록)
            child.SetParent(null);

            // 조금 더 ‘튀는’ 효과를 주고 싶다면 AddExplosionForce 등의 임의의 힘 추가
            // childRb.AddExplosionForce(300f, transform.position, 5f);
        }

        // 3) 자신(부모) 오브젝트를 일정 시간 뒤 파괴
        //    - 이미 MeshRenderer와 Collider를 껐으므로 시각적, 물리적으로는 사라진 상태
        Destroy(gameObject, 0.5f);
    }

}
