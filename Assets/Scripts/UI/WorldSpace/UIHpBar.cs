using System.Collections;
using Stats.BaseStats;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WorldSpace
{
    public class UIHpBar : UIBase
    {
        private readonly Vector3 _offsetHpbar = new Vector3(0, 1.5f,0);
        BaseStats _stats;
        Slider _hpSlider;
        CanvasGroup _canvasGroup;
        bool _isDamaged = false;
        enum HpBarSlider
        {
            HpBar
        }

        protected override void AwakeInit()
        {
            Bind<Slider>(typeof(HpBarSlider));
            _hpSlider = Get<Slider>((int)HpBarSlider.HpBar);
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;//처음에는 HP가 안보이게 설정 맞았을때 보이게끔 설정
        }

        protected override void StartInit()
        {
            _stats = GetComponentInParent<BaseStats>();
            transform.position = _stats.transform.position+ _offsetHpbar * (_stats.GetComponent<Collider>().bounds.size.y);
            _stats.EventAttacked += SetHpUI;
        }

        void LateUpdate()
        {
            if(_isDamaged)
                transform.rotation = Camera.main.transform.rotation;
        }
        public void SetHpUI(int damage, int currentHp)
        {
            _isDamaged = true;
            _hpSlider.value = (float)currentHp / (float)_stats.MaxHp;
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
}
