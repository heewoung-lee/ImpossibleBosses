using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;
public class UI_HPBar : UI_Base
{
    private readonly Vector3 OFFSET_HPBAR = new Vector3(0, 1.2f,0);
    BaseStats _stats;
    Slider _hpSlider;
    CanvasGroup _canvasGroup;
    bool _isDamaged = false;
    enum HPBarSlider
    {
        HPBar
    }

    protected override void AwakeInit()
    {
        Bind<Slider>(typeof(HPBarSlider));
        _hpSlider = Get<Slider>((int)HPBarSlider.HPBar);
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;//처음에는 HP가 안보이게 설정 맞았을때 보이게끔 설정
    }

    protected override void StartInit()
    {
        _stats = GetComponentInParent<BaseStats>();
        transform.position = _stats.transform.position+ OFFSET_HPBAR * (_stats.GetComponent<Collider>().bounds.size.y);
        _stats.Event_Attacked -= SetHpUI;
        _stats.Event_Attacked += SetHpUI;
    }

    void LateUpdate()
    {
        if(_isDamaged)
        transform.rotation = Camera.main.transform.rotation;
    }
    public void SetHpUI(int damage)
    {
        _isDamaged = true;
        _hpSlider.value = (float)_stats.Hp / (float)_stats.MaxHp;
        _canvasGroup.alpha = 1f;
        StopCoroutine(Hpbar_fadeaway());
        StartCoroutine(Hpbar_fadeaway());

    }

    IEnumerator Hpbar_fadeaway()
    {
        while (true)
        {
            _canvasGroup.alpha -= 0.3f * Time.deltaTime;

            if(_canvasGroup.alpha <= 0f)
            {
                _isDamaged = false;
                yield break;
            }
            yield return null;
        }
    }
}
