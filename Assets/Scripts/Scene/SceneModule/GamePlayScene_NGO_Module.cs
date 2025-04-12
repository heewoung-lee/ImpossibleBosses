using UnityEngine;

public class GamePlayScene_NGO_Module : MonoBehaviour
{
    private void Awake()
    {
    }
    protected void Start()
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost)
        {
            Managers.RelayManager.Load_NGO_Prefab<NGO_PlaySceneSpawn>();
            Managers.NGO_PoolManager.Create_NGO_Pooling_Object();//네트워크 오브젝트 풀링 생성
        }
    }
}
