using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameManagers;
using Unity.Netcode;
using UnityEngine;

public class Module_Boss_HP_UI : MonoBehaviour
{
    private void Start()
    {
        UI_Boss_HP boss_HP_UI = Managers.UIManager.GetSceneUIFromResource<UI_Boss_HP>();
    }

}
