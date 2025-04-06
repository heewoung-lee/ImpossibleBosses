using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NGO_UI_Stage_Timer : UI_Scene
{
    private const float villageStayTime = 300f;
    private const float boosRoomStayTime = 60f;

    private NetworkVariable<float> _timer = new NetworkVariable<float>
        (0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);

    private NetworkVariable<float> _timerFillAmount = new NetworkVariable<float>
      (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    private Image _timerDial;
    private TMP_Text _timerText;


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
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _timer.OnValueChanged -= onChangedTimerValue;
        _timer.OnValueChanged += onChangedTimerValue;
        _timerFillAmount.OnValueChanged -= OnChangedTimeFillAmount;
        _timerFillAmount.OnValueChanged += OnChangedTimeFillAmount;

        SetTimer();
    }

    private void OnChangedTimeFillAmount(float previousValue, float newValue)
    {
        _timerDial.fillAmount = newValue;
    }

    private void onChangedTimerValue(float previousValue, float newValue)
    {
        int second = (int)newValue % 60;
        int minute = (int)newValue / 60;

        _timerText.text = $"{minute} : {second:D2}"; 
    }

    public void SetTimer()
    {
        if (IsHost == false)
            return;


       BaseScene baseScene =  Managers.SceneManagerEx.GetCurrentScene;
        _timer.Value = baseScene.CurrentScene == Define.Scene.GamePlayScene ? villageStayTime : boosRoomStayTime;
        StartCoroutine(playCount(_timer.Value));
    }

    private IEnumerator playCount(float time)
    {
        float elapsedTime = time;
        int lastSecond = Mathf.FloorToInt(elapsedTime);
        float syncInterval = 0.1f;
        float timeSinceLastSync = 0f;

        while (elapsedTime > 0)
        {
            float delta = Time.deltaTime;
            elapsedTime -= delta;
            timeSinceLastSync += delta;

            if (timeSinceLastSync >= syncInterval)
            {
                _timerFillAmount.Value = Mathf.Clamp01(elapsedTime / time); // 0.1초마다 전송
                timeSinceLastSync = 0f;
            }

            int currentSecond = Mathf.FloorToInt(elapsedTime);
            if (currentSecond != lastSecond)
            {
                _timer.Value = currentSecond;
                lastSecond = currentSecond;
            }

            yield return null;
        }

        _timerFillAmount.Value = 0f;
        _timer.Value = 0f;

        //TODO:타이머 다 됐을때 실행할 동작 만들기
    }


}
