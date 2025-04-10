using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NGO_UI_Stage_Timer : UI_Scene
{
    private const float VillageStayTime = 300f;
    private const float BossRoomStayTime = 60f;
    private Color _normalClockColor = "FF9300".HexCodetoConvertColor();
    private Color _allPlayerInPortalColor = "0084FF".HexCodetoConvertColor();

    private NetworkVariable<float> _timer = new NetworkVariable<float>
        (0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);

    private NetworkVariable<float> _tmpTimer = new NetworkVariable<float>
      (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<float> _timerFillAmount = new NetworkVariable<float>
      (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<float> _tmpTimerFillAmount = new NetworkVariable<float>
     (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<bool> _isCheckAllPlayerinPortal = new NetworkVariable<bool>
        (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    private Image _timerDial;
    private TMP_Text _timerText;
    private Coroutine _playcount; 


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
        _isCheckAllPlayerinPortal.OnValueChanged -= OnChangedIscheckPlayerInPortal;
        _isCheckAllPlayerinPortal.OnValueChanged += OnChangedIscheckPlayerInPortal;

        _tmpTimerFillAmount.OnValueChanged += (oldvalue, newvalue) => Debug.Log(newvalue+"새로운값");
        SetTimer();
    }


    public void SetIscheckPlayerInPortal(bool ischeckAllPlayerInPortal)
    {
        _isCheckAllPlayerinPortal.Value = ischeckAllPlayerInPortal;
    }


    private void OnChangedIscheckPlayerInPortal(bool previousValue, bool newValue)
    {
        //새로운 밸류가 true이면,
        //시간을 보관해야하고,
        //7초로 다시 SetTimer()가 돌아가야한다.
        if(newValue == true)
        {
            _tmpTimer.Value = _timer.Value;
            _tmpTimerFillAmount.Value = _timerFillAmount.Value;
            _timer.Value = 7;
            _timerFillAmount.Value = 0;
            _timerDial.color = _allPlayerInPortalColor;
            StartTimer();
        }
        else
        {
            _timer.Value = _tmpTimer.Value;
            _timerFillAmount.Value = _tmpTimerFillAmount.Value;
            _timerDial.color = _normalClockColor;
            StartTimer();
        }

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

    private void SetTimer()
    {
        if (IsHost == false)
            return;

        BaseScene baseScene = Managers.SceneManagerEx.GetCurrentScene;
        _timer.Value = baseScene.CurrentScene == Define.Scene.GamePlayScene ? VillageStayTime : BossRoomStayTime;
      
        StartTimer();
    }



    private void StartTimer()
    {

        if (_playcount != null)
        {
            StopCoroutine(_playcount);
        }

        _playcount = StartCoroutine(playCount(_timer.Value));

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
