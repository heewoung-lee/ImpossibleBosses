using System.Collections;
using System.Collections.Generic;
using GameManagers;
using UnityEngine;

public class Module_Damage_Text : MonoBehaviour
{
    private void Start()
    {
        BaseStats stat = GetComponent<BaseStats>();
        stat.Event_Attacked += ShowDamageText_UI;
    }
    public void ShowDamageText_UI(int damage, int currentHp)
    {
        UI_DamageText uI_DamageText = Managers.UI_Manager.MakeUIWorldSpaceUI<UI_DamageText>();
        uI_DamageText.SetTextAndPosition(transform, damage);
        uI_DamageText.transform.SetParent(transform);
    }
}
