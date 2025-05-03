using UnityEngine;

public class StoneShatter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // ��: �÷��̾ �ٴڰ� �浹 �� ó��
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
