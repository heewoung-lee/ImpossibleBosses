using Unity.Netcode;
using UnityEngine;

public class NGO_Stage_Timer_Controller : NetworkBehaviourBase
{

    private const float VillageStayTime = 300f;
    private const float BossRoomStayTime = 60f;
    private const float AllPlayerinPortalCount = 7f;
    private float _totalCount = 0;
    private float _currenntCount = 0;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SetHostTimer();
        RequestTimeFromServerRpc();
    }

    private void SetHostTimer()
    {
        if (IsHost == false)
            return;

        BaseScene baseScene = Managers.SceneManagerEx.GetCurrentScene;
        _totalCount = baseScene.CurrentScene == Define.Scene.GamePlayScene ? VillageStayTime : BossRoomStayTime;
        Managers.UI_Manager.Get_Scene_UI<UI_Stage_Timer>().SetTimer(_totalCount);
    }


    protected override void AwakeInit()
    {
    }

    protected override void StartInit()
    {
    }

    [Rpc(SendTo.Server)]
    public void RequestTimeFromServerRpc(RpcParams rpcParams = default)
    {
        float currentCount = Managers.UI_Manager.Get_Scene_UI<UI_Stage_Timer>().CurrentTime;
        ulong clientId = rpcParams.Receive.SenderClientId;

        // 해당 클라이언트에게만 응답
        SendTimeRpcToSpecificClientRpc(currentCount, RpcTarget.Single(clientId, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void SendTimeRpcToSpecificClientRpc(float currentCount, RpcParams rpcParams = default)
    {
        _currenntCount = currentCount;
        Debug.Log($"클라이언트가 받은 서버 시간: {currentCount}");
        Managers.UI_Manager.Get_Scene_UI<UI_Stage_Timer>().SetTimer(_totalCount, _currenntCount);
    }


}
