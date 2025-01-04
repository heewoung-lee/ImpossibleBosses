using System;
using UnityEngine;

[Serializable]
public class StatEffect
{
    public StatEffect(StatType statType, float value, string buffname)
    {
        this.statType = statType;
        this.value = value;
        this.buffname = buffname;
    }

    public StatType statType;  // ��ȭ ����
    public float value;        // ��ȭ ��
    public string buffname;
}
