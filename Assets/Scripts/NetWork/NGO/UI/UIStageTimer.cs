using System;
using System.Collections;
using TMPro;
using UI.Scene;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace NetWork.NGO.UI
{
    public class UIStageTimer : UIScene
    {

        private Image _timerDial;
        private TMP_Text _timerText;
        private Coroutine _playcountCoroutine;

        private float _totalCount;
        private float _currentTime;
        private float _timerFillAmount;

        private Action _onTimerCompleted;
        public event Action OnTimerCompleted
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _onTimerCompleted, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _onTimerCompleted, value);
            }
        }


        public float TimerFillAmount
        {
            get
            {
                return _timerFillAmount;
            }
            private set
            {
                _timerDial.fillAmount = value;
            }
        }

        public float CurrentTime => _currentTime;

        enum TimerImage
        {
            TimeDial
        }
        enum TimerText
        {
            Timer_Text
        }


        protected override void AwakeInit()
        {
            base.AwakeInit();
            Bind<Image>(typeof(TimerImage));
            Bind<TMP_Text>(typeof(TimerText));
            _timerDial = Get<Image>((int)TimerImage.TimeDial);
            _timerText = Get<TMP_Text>((int)TimerText.Timer_Text);
            _timerFillAmount = _timerDial.fillAmount;   
        }

        public void SetTimer(float totalCount,Color counterColor = default)
        {
            if (counterColor.Equals(default) == false)
            {
                _timerDial.color = counterColor;
            }
            _currentTime = totalCount;
            _totalCount = totalCount;

            if(_playcountCoroutine != null)
                StopCoroutine( _playcountCoroutine);

            _playcountCoroutine = StartCoroutine(playCount());
        }
        public void SetTimer(float totalCount,float currentCount, Color counterColor = default)
        {
            if (counterColor.Equals(default) == false)
            {
                _timerDial.color = counterColor;
            }
            _currentTime = currentCount;
            _totalCount = totalCount;

            if (_playcountCoroutine != null)
                StopCoroutine(_playcountCoroutine);

            _playcountCoroutine = StartCoroutine(playCount());
        }

        private void onChangedTimerValue(float currentTime)
        {
            int second = (int)currentTime % 60;
            int minute = (int)currentTime / 60;

            _timerText.text = $"{minute} : {second:D2}"; 
        }

        private IEnumerator playCount()
        {

            int lastSecond = Mathf.FloorToInt(_currentTime);

            while (_currentTime > 0)
            {
                _currentTime -= Time.unscaledDeltaTime;
                TimerFillAmount = Mathf.Clamp01(_currentTime / _totalCount);
                onChangedTimerValue(_currentTime);
                yield return null;
            }

            _timerFillAmount = 0f;
            _currentTime = 0f;

            _onTimerCompleted?.Invoke();
        }

    }
}
