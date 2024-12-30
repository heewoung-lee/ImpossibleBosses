using System.Collections;
using UnityEngine;

public class CanonTest : MonoBehaviour
{
    public Transform Target; // 목표 지점 (플레이어)
    public Transform startPoint; // 발사 위치
    public Transform targetPoint; // 목표 위치
    public float firingAngle = 45.0f; // 발사 각도

    private void Start()
    {
        // 타겟과 시작점을 설정
        Target = Managers.GameManagerEx.Player.transform;
        startPoint = transform;
    }

    void Update()
    {
        // 스페이스 키를 눌렀을 때 발사
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LaunchProjectile();
        }
    }

    void LaunchProjectile()
    {
        // 발사체 생성
        GameObject projectile = Managers.ResourceManager.InstantiatePrefab("Enemy/Boss/AttackPattren/BossSkill1");
        projectile.transform.position = Target.position + Vector3.up * 5f;


        Managers.ResourceManager.DestroyObject(projectile, 2f);
    }
}
