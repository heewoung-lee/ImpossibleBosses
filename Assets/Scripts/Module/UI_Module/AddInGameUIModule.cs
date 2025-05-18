using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AddInGameUIModule : MonoBehaviour
{
    void Start()
    {
        gameObject.AddComponent<Module_UI_BufferBar>();
        gameObject.AddComponent<Module_UI_ConsumableBar>();
        gameObject.AddComponent<Module_UI_ItemDragImage>();
        gameObject.AddComponent<Module_UI_Player_Inventory>();
        gameObject.AddComponent<Module_UI_PlayerInfo>();
        gameObject.AddComponent<Module_UI_SkillBar>();
        gameObject.AddComponent<Module_UI_Description>();

        Managers.NGO_PoolManager.Create_NGO_Pooling_Object();

        gameObject.AddComponent<Module_UI_Player_TestButton>();

    }
}
