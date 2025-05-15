using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class DropItemBehaviour : MonoBehaviour,LootItemBehaviour
{
    private readonly float TORQUE_FORCE_OFFSET = 10f;
    private readonly float MAX_HEIGHT = 3f;
    private readonly float CircleRange = 30f;
    private readonly float itemFlightDuration = 1.5f;
    public void SpawnBahaviour(Rigidbody rigid)
    {

        Vector2 randomCircle = Random.insideUnitCircle * CircleRange;
        Vector3 offset = new Vector3(randomCircle.x, 0f, randomCircle.y);
        Vector3 spawnPos = rigid.transform.position + offset;
        transform.position = rigid.transform.position + Vector3.up * 1.2f;

        Vector3 randomTorque = new Vector3(
           Random.Range(-1f, 1f),  // X�� ȸ��
           Random.Range(-1f, 1f),  // Y�� ȸ��
           Random.Range(-1f, 1f)   // Z�� ȸ��
       );
        // ȸ�� �� �߰� (���� ���� ������ ����)
        rigid.AddTorque(randomTorque * TORQUE_FORCE_OFFSET, ForceMode.Impulse);
        StartCoroutine(ThrowStoneParabola(rigid.transform, spawnPos, itemFlightDuration));
       
    }
    public IEnumerator ThrowStoneParabola(Transform lootItemTr, Vector3 targetPos, float duration)
    {
        Vector3 startPos = lootItemTr.position;
        Vector3 targetPoint = targetPos;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // t: ���� ���� (0~1)
            float t = elapsedTime / duration;

            // XZ ��ġ ����
            Vector3 currentXZ = Vector3.Lerp(startPos, targetPoint, t);

            // Y ���� ������ ���
            float currentY = Mathf.Lerp(startPos.y, targetPoint.y, t) + MAX_HEIGHT * Mathf.Sin(Mathf.PI * t);

            // ���� ��ġ ����
            lootItemTr.position = new Vector3(currentXZ.x, currentY, currentXZ.z);

            yield return null;
        }
    }

}