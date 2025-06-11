using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
using GameManagers;
using Unity.Netcode;
using UnityEngine;

public class NGO_UI_Root_Chracter_Select : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsHost == false)
            return;

        transform.SetParent(Managers.RelayManager.NgoRootUI.transform);
        Managers.UIManager.Get_Scene_UI<UI_Room_CharacterSelect>().Set_NGO_UI_Root_Character_Select(this.transform);
    }
}
