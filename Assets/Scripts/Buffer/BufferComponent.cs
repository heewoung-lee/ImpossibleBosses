using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Buffer
{
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

        private BuffModifier _modifier;
        public BuffModifier Modifier { get => _modifier; }

        private Coroutine _bufferCoroutine; 
 

        private void InitAndStartBuffInternal(BaseStats targetStat, float duration)
        {
            _targetStat = targetStat;
            _duration = duration;
            _image = GetComponentInChildren<Image>();
            if (_modifier is ImmediatelyBuff)
            {
                _image.sprite = null;
            }
            else
            {
                _image.sprite = ((DurationBuff)_modifier).BuffIconImage;
            }
            StartBuff();
        }


        public void InitAndStartBuff(BaseStats targetStat, float duration,StatEffect effect)
        {
            _modifier = Managers.BufferManager.GetModifier(effect);
            _value = effect.value;
            InitAndStartBuffInternal(targetStat, duration);
        }
        public void InitAndStartBuff(BaseStats targetStat, float duration, BuffModifier buff_Modifier,float value)
        {
            _modifier = buff_Modifier;
            _value = value;
            InitAndStartBuffInternal(targetStat, duration);
        }

        private void StartBuff()
        {
            if (_modifier is ImmediatelyBuff)
            {
                Managers.BufferManager.ImmediatelyBuffStart(this);
                return;
            }
            else if(_modifier is DurationBuff)
            {
                if (RemoveSameTypeBuff() == false)
                {
                    _bufferCoroutine = StartBuffer();
                }
            }
        }

        private bool RemoveSameTypeBuff()
        {
            foreach (BufferComponent buffer in transform.parent.GetComponentsInChildren<BufferComponent>())
            {

                if (_modifier != buffer.Modifier)//같은 버프가 아니라면 넘긴다.
                    continue;

                if (buffer == this)//내 버프라면 넘긴다.
                    continue;

                else
                {
                    buffer.BufferReStart();
                    Managers.ResourceManager.DestroyObject(gameObject);
                    return true;
                }
            }
            return false;
        }

        private IEnumerator StartBuffFlicker()
        {
            if (_image != null)
            {
                float elapsedTime = _duration;
                float minAlpha = 0.3f; // 최소 알파값 (조절 가능)
                float maxAlpha = 1f;   // 최대 알파값 (조절 가능)
                float remainingTime = 5f;
                float timeDeal = 0f;
                // 처음에는 이미지를 완전한 알파값으로 세팅
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
                        timeDeal += Time.deltaTime * flickerSpeed;//값의 증가량을 일정하게 높여야하므로 Time을 더함
                        float t = Mathf.PingPong(timeDeal, 1f);
                        color.a = Mathf.Lerp(minAlpha, maxAlpha, t);
                        _image.color = color;
                    }
                    else
                    {
                        // 5초 이상 남았을 땐 깜빡이지 않고 알파값을 고정
                        color = _image.color;
                        color.a = maxAlpha;
                        _image.color = color;
                    }
                    yield return null;
                }
            }
            Managers.BufferManager.RemoveBuffer(this);
        }



        private Coroutine StartBuffer()
        {
            _modifier.ApplyStats(_targetStat, _value);
            return StartCoroutine(StartBuffFlicker());
        }


        public void BufferReStart()
        {
            if (_bufferCoroutine != null)
            {
                StopCoroutine(_bufferCoroutine);
            }

            _bufferCoroutine = StartCoroutine(StartBuffFlicker());
        }
    }
}
