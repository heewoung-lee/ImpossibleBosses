using System.Collections.Generic;
using GameManagers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AddInGameUIModule : MonoBehaviour
{

    private void Start()
    {
        StartInit();
    }

    protected virtual void StartInit()
    {
        gameObject.AddComponent<Module_UI_BufferBar>();
        gameObject.AddComponent<Module_UI_ConsumableBar>();
        gameObject.AddComponent<Module_UI_ItemDragImage>();
        gameObject.AddComponent<Module_UI_Player_Inventory>();
        gameObject.AddComponent<Module_UI_PlayerInfo>();
        gameObject.AddComponent<Module_UI_SkillBar>();
        gameObject.AddComponent<Module_UI_Description>();

        Managers.NgoPoolManager.Create_NGO_Pooling_Object();

    }
}
