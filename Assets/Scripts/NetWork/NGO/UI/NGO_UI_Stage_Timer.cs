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
    private const float AllPlayerinPortalCount = 7f;
    private Color _normalClockColor = "FF9300".HexCodetoConvertColor();
    private Color _allPlayerInPortalColor = "0084FF".HexCodetoConvertColor();

    private NetworkVariable<float> _timer = new NetworkVariable<float>
        (0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);

    private NetworkVariable<float> _tmpTimer = new NetworkVariable<float>
      (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<float> _timerFillAmount = new NetworkVariable<float>
      (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<bool> _isCheckAllPlayerinPortal = new NetworkVariable<bool>
        (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<Color> _timerColor = new NetworkVariable<Color>
        ("FF9300".HexCodetoConvertColor(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    private Image _timerDial;
    private TMP_Text _timerText;
    private Coroutine _playcountCoroutine; 


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
        _timerColor.OnValueChanged -= OnChangedTimerColor;
        _timerColor.OnValueChanged += OnChangedTimerColor;

        SetTimer();
    }

    private void OnChangedTimerColor(Color previousValue, Color newValue)
    {
        _timerDial.color = newValue;
    }

    public void SetIscheckPlayerInPortal(bool ischeckAllPlayerInPortal)
    {
        _isCheckAllPlayerinPortal.Value = ischeckAllPlayerInPortal;
    }


    private void OnChangedIscheckPlayerInPortal(bool previousValue, bool newValue)
    {

        //���ο� ����� true�̸�,
        //�ð��� �����ؾ��ϰ�,
        //7�ʷ� �ٽ� SetTimer()�� ���ư����Ѵ�.
        if (newValue == true)
        {
            AllPlayerInPortalAndStartCount();
        }
        else
        {
            ReturnToNormalCountdown();
        }

        void AllPlayerInPortalAndStartCount()
        {
            if (IsHost == false)
                return;

            _tmpTimer.Value = _timer.Value;//�ӽõ����Ϳ� ����

            _timer.Value = AllPlayerinPortalCount;
            _timerFillAmount.Value = 0;
            _timerColor.Value = _allPlayerInPortalColor;
            StartTimer(AllPlayerinPortalCount);
        }
        void ReturnToNormalCountdown()
        {
            if (IsHost == false)
                return;
            _timer.Value = _tmpTimer.Value;
            _timerColor.Value = _normalClockColor;
            StartTimer(VillageStayTime);
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
      
        StartTimer(VillageStayTime);
    }



    private void StartTimer(float totalTimer)
    {

        if (_playcountCoroutine != null)
        {
            StopCoroutine(_playcountCoroutine);
        }

        _playcountCoroutine = StartCoroutine(playCount(totalTimer, _timer.Value));

    }

    private IEnumerator playCount(float totalTimer,float time)
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
                _timerFillAmount.Value = Mathf.Clamp01(elapsedTime / totalTimer); // 0.1�ʸ��� ����
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

        //TODO:Ÿ�̸� �� ������ ������ ���� �����
    }


}
