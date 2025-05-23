using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GamePlaySceneUnit : IGamePlaySceneSpawnBehaviour
{
    private UI_Stage_Timer _ui_stage_timer;
    private UI_Loading _ui_Loading_Scene;
    private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;

    public void Init()
    {
        _ui_Loading_Scene = Managers.UI_Manager.GetOrCreateSceneUI<UI_Loading>();
        _ui_stage_timer = Managers.UI_Manager.GetOrCreateSceneUI<UI_Stage_Timer>();
        _ui_stage_timer.OnTimerCompleted += MoveToBattleScene;
    }
    public void MoveToBattleScene()//ȣ��Ʈ���Ը� �����.
    {
        Managers.RelayManager.NGO_RPC_Caller.ResetManagersRpc();
        Managers.RelayManager.NetworkManagerEx.NetworkConfig.EnableSceneManagement = true;
        Managers.SceneManagerEx.NetworkLoadScene(Define.Scene.BattleScene, ClientLoadedEvent, () => { });
        void ClientLoadedEvent(ulong clientId)
        {
            Debug.Log($"{clientId} �÷��̾� �ε� �Ϸ�");

            foreach (NetworkObject clicentNgoObj in Managers.RelayManager.NetworkManagerEx.SpawnManager.SpawnedObjectsList)
            {
                if (clicentNgoObj.OwnerClientId != clientId)
                {
                    continue;
                }
                if (clicentNgoObj.TryGetComponent(out PlayerStats playerStats) == true)
                {
                    Debug.Log($"{clientId}�÷��̾� ã�Ҵ�");
                    playerStats.transform.SetParent(Managers.RelayManager.NGO_ROOT.transform);
                    playerStats.transform.position = new Vector3(clientId, 0, 0);
                    break;
                }
            }
        }

    }

    public void SpawnOBJ()
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost)
        {
            Managers.RelayManager.Load_NGO_Prefab<NGO_GamePlaySceneSpawn>();
        }
    }
}
