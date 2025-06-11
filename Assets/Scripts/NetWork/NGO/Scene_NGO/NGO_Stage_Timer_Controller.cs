using GameManagers;
using Unity.Netcode;
using UnityEngine;

public class NGO_Stage_Timer_Controller : NetworkBehaviour
{
    private Color _normalClockColor = "FF9300".HexCodetoConvertColor();
    private Color _allPlayerInPortalColor = "0084FF".HexCodetoConvertColor();


    private const float VillageStayTime = 300f;
    //private const float BossRoomStayTime = 60f;
   private const float BossRoomStayTime = 1f;
    private const float AllPlayerinPortalCount = 7f;
    private float _totalTime = 0;
    private float _currentTime = 0;

    private UI_Stage_Timer _ui_Stage_Timer;

    public UI_Stage_Timer UI_Stage_Timer
    {
        get
        {
            if (_ui_Stage_Timer == null)
            {
                _ui_Stage_Timer = Managers.UI_Manager.GetOrCreateSceneUI<UI_Stage_Timer>();
            }
            return _ui_Stage_Timer;
        }
    }

    public float TotalTime
    {
        get
        {
            if (_totalTime == default)
            {
                Define.Scene currentScene = Managers.SceneManagerEx.CurrentScene;
                _totalTime = currentScene == Define.Scene.GamePlayScene ? VillageStayTime : BossRoomStayTime;
            }
            return _totalTime;  
        }
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SetHostTimer();
        if (IsHost == false) //클라이언트가 서버에 도는 시간을 가져와야 한다.
        {
            RequestTimeFromServerRpc();
        }
    }

    private void SetHostTimer()
    {
        if (IsHost == false)
            return;

        UI_Stage_Timer.SetTimer(TotalTime);
    }


    [Rpc(SendTo.Server)]
    private void RequestTimeFromServerRpc(RpcParams rpcParams = default)
    {
        float currentCount = Managers.UI_Manager.Get_Scene_UI<UI_Stage_Timer>().CurrentTime;
        ulong clientId = rpcParams.Receive.SenderClientId;

        SendTimeRpcToSpecificClientRpc(currentCount, RpcTarget.Single(clientId, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void SendTimeRpcToSpecificClientRpc(float currentCount, RpcParams rpcParams = default)
    {
        _currentTime = currentCount;
        UI_Stage_Timer.SetTimer(TotalTime, _currentTime);
    }


    [Rpc(SendTo.ClientsAndHost)]
    public void SetPortalInAllPlayersCountRpc()
    {
        _currentTime = UI_Stage_Timer.CurrentTime;
        UI_Stage_Timer.SetTimer(AllPlayerinPortalCount, _allPlayerInPortalColor);
    }


    [Rpc(SendTo.ClientsAndHost)]
    public void SetNormalCountRpc()
    {
        UI_Stage_Timer.SetTimer(TotalTime, _currentTime, _normalClockColor);
    }
}
