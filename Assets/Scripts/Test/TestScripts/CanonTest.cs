using System.Collections;
using GameManagers;
using GameManagers.Interface.Resources_Interface;
using UnityEngine;
using Zenject;

namespace Test.TestScripts
{
    public class CanonShooter : MonoBehaviour
    {
        [Inject] private IInstantiate _instantiate;
        [Inject] IDestroyObject _destroyer;
        [Inject] GameManagerEx _gameManagerEx;
        public Transform startTransform;  // 시작 위치
        public Transform targetTransform; // 목표 위치
        public GameObject projectilePrefab; // 발사체 프리팹
        public float maxHeight = 5f;    // 최대 높이
        public float flightSpeed = 8f; // 비행 속도 (거리 기반)

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Launch();
            }
        }

        private void Start()
        {
            startTransform = transform; 
        }
        public void Launch()
        {
            targetTransform = _gameManagerEx.Player.transform;

            // 발사체 생성
            GameObject projectile = _instantiate.InstantiateByPath("Prefabs/Enemy/Boss/AttackPattren/BossSkill1");

            projectile.transform.SetParent(Managers.VFXManager.VFXRootNgo);
            projectile.transform.position += Vector3.up * GetComponent<Collider>().bounds.max.y;
            projectile.transform.rotation = Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));

            // 코루틴 시작
            StartCoroutine(Parabola(projectile.transform));
        }

        private IEnumerator Parabola(Transform projectile)
        {
            Vector3 startPoint = startTransform.position + projectile.transform.position;
            Vector3 targetPoint = targetTransform.position;

            // 거리 기반 비행 시간 계산
            float distance = Vector3.Distance(startPoint, targetPoint);
            //float duration = distance/ flightSpeed;
            float duration = 1f;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                // t: 진행 비율 (0~1)
                float t = elapsedTime / duration;

                // XZ 위치 보간
                Vector3 currentXZ = Vector3.Lerp(startPoint, targetPoint, t);

                // Y 값은 포물선 계산
                float currentY = Mathf.Lerp(startPoint.y, targetPoint.y, t) +
                                 maxHeight * Mathf.Sin(Mathf.PI * t);

                // 최종 위치 설정
                projectile.position = new Vector3(currentXZ.x, currentY, currentXZ.z);

                yield return null;
            }

            // 포물선 이동 완료 후 파괴
            _destroyer.DestroyObject(projectile.gameObject, 2f);
        }
    }
}
