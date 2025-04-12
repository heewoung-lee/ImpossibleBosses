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
            Managers.NGO_PoolManager.Create_NGO_Pooling_Object();//��Ʈ��ũ ������Ʈ Ǯ�� ����
        }
    }
}
