using System.Collections;
using GameManagers;
using GameManagers.Interface.GameManagerEx;
using Stats.BossStats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Scene.SceneUI
{
    public class UIBossHp : UIScene
    {
        [Inject] IBossSpawnManager _bossSpawnManager;
        enum HpSlider
        {
            BossHpSlider
        }
        enum HpText
        {
            HpText
        }
    
        private Slider _hpSlider;
        private TMP_Text _hpText;
        private BossStats _stats;
        private int _currentHp;
        protected override void AwakeInit()
        {
            base.AwakeInit();
            Bind<Slider>(typeof(HpSlider));
            Bind<TMP_Text>(typeof(HpText));

            _hpText = GetText((int)HpText.HpText);
            _hpSlider = Get<Slider>((int)HpSlider.BossHpSlider);
        }

        protected override void StartInit()
        {

            if(_bossSpawnManager.GetBossMonster() != null)
            {
                SetBossStatUI();
            }
            else
            {
                _bossSpawnManager.OnBossSpawnEvent += SetBossStatUI;
            }

            void SetBossStatUI()
            {
                _stats = _bossSpawnManager.GetBossMonster().GetComponent<BossStats>();
                _stats.CurrentHpValueChangedEvent += Stats_CurrentHPValueChangedEvent;
                _stats.MaxHpValueChangedEvent += Stats_CurrentMAXHPValueChangedEvent;

                if (_stats.MaxHp <= 0)
                    return;

                _hpText.text = $"{_stats.Hp} / {_stats.MaxHp}";
            }
        }

        private void Stats_CurrentMAXHPValueChangedEvent(int preCurrentMaxHp, int currentMaxHp)
        {
            _hpText.text = $"{_stats.Hp} / {currentMaxHp}";
            _hpSlider.value = (float)_stats.Hp / (float)currentMaxHp;
        }

        private void Stats_CurrentHPValueChangedEvent(int preCurrentHp, int currentHp)
        {
            if (_stats.MaxHp <= 0)
                return;
            StartCoroutine(AnimationHp(preCurrentHp- currentHp));
            _hpText.text = $"{currentHp} / {_stats.MaxHp}";
        }

        private IEnumerator AnimationHp(int damage)
        {
            float duration = 1.0f;
            float elapsedTime = 0f;
            float beforeHp = ((float)_stats.Hp+ damage) / (float)_stats.MaxHp;
            float afterHp = ((float)_stats.Hp) / (float)_stats.MaxHp;

            while(elapsedTime < duration)
            {
                //흘러가는 경과시간
                elapsedTime += Time.deltaTime * 3f;
                _hpSlider.value = Mathf.Lerp(beforeHp, afterHp, elapsedTime);
                yield return null;
            }
            _hpSlider.value = afterHp;
        }

  
    }
}
