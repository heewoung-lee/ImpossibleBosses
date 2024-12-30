using UnityEngine;

public class StoneShatter : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        // ��: �÷��̾ �ٴڰ� �浹 �� ó��
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Ground"))
        {
            Shatter();
        }
    }


    void Shatter()
    {
        // 1) �θ� �ڽ��� MeshRenderer/Collider ���� (�Ǵ� �ı�)
        //    - �ð����� �� ������Ʈ�� �������, ���� ������� ���̵��� �� �� ����.
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        // �ʿ��ϴٸ� Rigidbody�� ���ų� Destroy�ع��� �� ����
        // Destroy(GetComponent<Rigidbody>());

        // 2) �ڽ� ����� ������ ���� ���� Ȱ��ȭ
        foreach (Transform child in transform)
        {
            Rigidbody childRb = child.GetComponent<Rigidbody>();
            Collider childCol = child.GetComponent<Collider>();
            if (childRb != null)
            {
                // isKinematic�� false�� �ٲ� ���� �ùķ��̼� ����
                childRb.isKinematic = false;
            }
            if (childCol != null)
            {
                childCol.enabled = true;
            }

            // �θ𿡼� �и�(�����Ӱ� �����̵���)
            child.SetParent(null);

            // ���� �� ��Ƣ�¡� ȿ���� �ְ� �ʹٸ� AddExplosionForce ���� ������ �� �߰�
            // childRb.AddExplosionForce(300f, transform.position, 5f);
        }

        // 3) �ڽ�(�θ�) ������Ʈ�� ���� �ð� �� �ı�
        //    - �̹� MeshRenderer�� Collider�� �����Ƿ� �ð���, ���������δ� ����� ����
        Destroy(gameObject, 0.5f);
    }

}
