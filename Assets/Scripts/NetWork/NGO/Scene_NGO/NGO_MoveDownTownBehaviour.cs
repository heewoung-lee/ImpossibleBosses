using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NGO_MoveDownTownBehaviour : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(Managers.UI_Manager.Try_Get_Scene_UI(out UI_Boss_HP bossHP))
        {
            Managers.ResourceManager.DestroyObject(bossHP.gameObject);
        }
        PlayerMoveSceneDownTownBehaviour();
    }


    public void PlayerMoveSceneDownTownBehaviour()
    {
        Managers.RelayManager.NetworkManagerEx.SceneManager.OnLoadComplete += SceneMoveInitalize;
        void SceneMoveInitalize(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            if (sceneName != Define.Scene.GamePlayScene.ToString() || loadSceneMode != LoadSceneMode.Single)
                return;

            if(clientId == Managers.RelayManager.NetworkManagerEx.LocalClientId)
            {
                Managers.Clear();//씬이 이동되면 모든 UI 초기화
            }
        }
    }
}
