using UnityEngine;

public class BallTest : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.layer);
    }
}
