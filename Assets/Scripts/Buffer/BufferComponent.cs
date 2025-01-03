using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BufferComponent : MonoBehaviour
{

    private BaseStats _targetStat;
    public BaseStats TarGetStat { get => _targetStat; }


    private List<BaseStats> _targetStats;
    public List<BaseStats> TargetStats { get => _targetStats; }

    private float _duration;

    private Image _image;

    private float _value;
    public float Value { get => _value; }

    private Buff_Modifier _modifier;
    public Buff_Modifier Modifier { get => _modifier; }
 

    public void InitAndStartBuff(BaseStats targetStat, float duration,ItemEffect effect)
    {
        _targetStat = targetStat;
        _duration = duration;
        _image = GetComponentInChildren<Image>();
        _modifier = Managers.BufferManager.GetModifier(effect);
        if(_modifier is Immediately_Buff)
        {
            _image.sprite = null;
        }
        else
        {
            _image.sprite = (_modifier as Duration_Buff).BuffIconImage;
        }
        _image.SetNativeSize();
        _value = effect.value;


        StartBuff();
    }


    private void StartBuff()
    {
        if (_modifier is Immediately_Buff)
        {
            Managers.BufferManager.ImmediatelyBuffStart(this);
            return;
        }
        else if(_modifier is Duration_Buff)
        {
            RemoveSameTypeBuff();
            StartCoroutine(StartBuffer());
        }
    }

    private void RemoveSameTypeBuff()
    {
        foreach (BufferComponent buffer in transform.parent.GetComponentsInChildren<BufferComponent>())
        {

            if (_modifier.Buffname != buffer.Modifier.Buffname)//���� ������ �ƴ϶�� �ѱ��.
                continue;

            if (buffer == this)//�� ������� �ѱ��.
                continue;

            else
            {
                Managers.BufferManager.RemoveBuffer(buffer);//���� ������ �ִٸ� ���� �ִ� ������ �����Ų��.
                break;
            }
        }
    }

    private IEnumerator StartBuffer()
    {
        _modifier.ApplyStats(_targetStat, _value);

        if (_image != null)
        {
            float elapsedTime = _duration;
            float minAlpha = 0.3f; // �ּ� ���İ� (���� ����)
            float maxAlpha = 1f;   // �ִ� ���İ� (���� ����)
            float remainingTime = 5f;
            float TimeDeal = 0f;
            // ó������ �̹����� ������ ���İ����� ����
            Color color = _image.color;
            color.a = maxAlpha;
            _image.color = color;

            while (elapsedTime > 0)
            {
                elapsedTime -= Time.deltaTime;

                if (elapsedTime < remainingTime)
                {
                    float timeRatio = 1f - (elapsedTime / remainingTime);
                    float flickerSpeed = Mathf.Lerp(3f, 10f, timeRatio);
                    TimeDeal += Time.deltaTime * flickerSpeed;//���� �������� �����ϰ� �������ϹǷ� Time�� ����
                    float t = Mathf.PingPong(TimeDeal, 1f);
                    color.a = Mathf.Lerp(minAlpha, maxAlpha, t);
                    _image.color = color;
                }
                else
                {
                    // 5�� �̻� ������ �� �������� �ʰ� ���İ��� ����
                    color = _image.color;
                    color.a = maxAlpha;
                    _image.color = color;
                }
                yield return null;
            }
        }
        Managers.BufferManager.RemoveBuffer(this);
    }

}
