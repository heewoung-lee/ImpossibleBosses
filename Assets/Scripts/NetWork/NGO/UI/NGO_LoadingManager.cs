using Unity.Netcode;
using UnityEngine;

public class NGO_LoadingManager : NetworkBehaviour
{
    private NetworkVariable<int> loadingProgress = new NetworkVariable<int>(0);
    private NetworkVariable<bool> isGameStarting = new NetworkVariable<bool>(false);
    private int totalPlayers;
    private int loadedPlayers = 0;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if (IsHost)
        {
            totalPlayers = NetworkManager.Singleton.ConnectedClients.Count;
        }

        loadingProgress.OnValueChanged += OnLoadingProgressChanged;
    }

    public void StartGame()
    {
        if (!IsHost) return;
        
        isGameStarting.Value = true;
        // 여기에 씬 전환 로직 추가
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReportLoadingCompleteServerRpc()
    {
        loadedPlayers++;
        loadingProgress.Value = (int)((float)loadedPlayers / totalPlayers * 100);
        
        if (loadedPlayers == totalPlayers)
        {
            // 모든 플레이어가 로딩 완료
            // 다음 씬으로 전환
        }
    }

    private void OnLoadingProgressChanged(int previous, int current)
    {
        // UI 업데이트 로직
        Managers.UI_Manager.Get_Scene_UI<UI_Loading>().UpdateLoadingProgress(current);
    }
} 