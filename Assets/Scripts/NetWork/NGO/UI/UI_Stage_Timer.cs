using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UI_Stage_Timer : UI_Scene
{
    private Color _normalClockColor = "FF9300".HexCodetoConvertColor();
    private Color _allPlayerInPortalColor = "0084FF".HexCodetoConvertColor();


    private Image _timerDial;
    private TMP_Text _timerText;
    private Coroutine _playcountCoroutine;

    private float _totalCount;
    [SerializeField]private float _currentTime;
    [SerializeField] private float _timerFillAmount;

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

    public void SetTimer(float totalCount)
    {
        _currentTime = totalCount;

        _playcountCoroutine = StartCoroutine(playCount());
       
    }
    public void SetTimer(float totalCount,float currentCount)
    {
        _currentTime = currentCount;
        playCount();
    }

    private void OnChangedTimerColor(Color previousValue, Color newValue)
    {
        _timerDial.color = newValue;
    }

    private void onChangedTimerValue(float previousValue, float newValue)
    {
        int second = (int)newValue % 60;
        int minute = (int)newValue / 60;

        _timerText.text = $"{minute} : {second:D2}"; 
    }

    private IEnumerator playCount()
    {

        int lastSecond = Mathf.FloorToInt(_currentTime);

        while (_currentTime > 0)
        {
            float delta = Time.deltaTime;
            _currentTime -= delta;
            _timerFillAmount = Mathf.Clamp01(_currentTime / _totalCount); // 0.1초마다 전송

            int currentSecond = Mathf.FloorToInt(_currentTime);
            if (currentSecond != lastSecond)
            {
                _currentTime = currentSecond;
                lastSecond = currentSecond;
            }

            yield return null;
        }

        _timerFillAmount = 0f;
        _currentTime = 0f;

        //TODO:타이머 다 됐을때 실행할 동작 만들기
    }


}
