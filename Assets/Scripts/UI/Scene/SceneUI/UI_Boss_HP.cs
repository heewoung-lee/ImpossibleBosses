using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Boss_HP : UI_Scene
{
    enum HP_Slider
    {
        Boss_HP_Slider
    }
    enum HP_Text
    {
        HP_Text
    }
    
    private Slider _hp_Slider;
    private TMP_Text _hp_Text;
    private BossStats _stats;
    protected override void AwakeInit()
    {
        base.AwakeInit();
        Bind<Slider>(typeof(HP_Slider));
        Bind<TMP_Text>(typeof(HP_Text));

        _hp_Text = GetText((int)HP_Text.HP_Text);
        _hp_Slider = Get<Slider>((int)HP_Slider.Boss_HP_Slider);
    }

    protected override void StartInit()
    {
        if(Managers.GameManagerEx.BossMonster != null)
        {
            SetBossStatUI();
        }
        else
        {
           Managers.GameManagerEx.OnBossSpawnEvent += SetBossStatUI;
        }

        void SetBossStatUI()
        {
            _stats = Managers.GameManagerEx.BossMonster.GetComponent<BossStats>();
            _hp_Text.text = $"{_stats.Hp} / {_stats.MaxHp}";

            _stats.Event_Attacked -= SetHpUI;
            _stats.Event_Attacked += SetHpUI;
        }
    }

    public void SetHpUI(int damage, int currentHp)
    {
        StartCoroutine(AnimationHP(damage));
        _hp_Text.text = $"{currentHp} / {_stats.MaxHp}";
    }


    IEnumerator AnimationHP(int damage)
    {
        float duration = 1.0f;
        float elapsedTime = 0f;
        float beforeHP = ((float)_stats.Hp+ damage) / (float)_stats.MaxHp;
        float afterHp = ((float)_stats.Hp) / (float)_stats.MaxHp;

        while(elapsedTime < duration)
        {
            //흘러가는 경과시간
            elapsedTime += Time.deltaTime * 3f;
            _hp_Slider.value = Mathf.Lerp(beforeHP, afterHp, elapsedTime);
            yield return null;
        }
        _hp_Slider.value = afterHp;
    }

  
}
