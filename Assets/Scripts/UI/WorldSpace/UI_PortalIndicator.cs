using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_PortalIndicator : UI_Base
{
    enum Images
    {
        PortalIndicatorIMG
    }


    private Image _indicatorIMG;

    private void OnDestroy()
    {
        Debug.Log($"{System.Environment.StackTrace}온디스트로이");
    }


    public void SetIndicatorOff()
    {
        _indicatorIMG.gameObject.SetActive(false);
    }
    public void SetIndicatorOn()
    {
        _indicatorIMG.gameObject.SetActive(true);
    }

    protected override void AwakeInit()
    {
        Bind <Image>(typeof(Images));
        _indicatorIMG = Get<Image>((int)Images.PortalIndicatorIMG);

        Managers.RelayManager.NetworkManagerEx.SceneManager.OnLoadEventCompleted += OnChangeSceneEvent;
    }

    private void OnChangeSceneEvent(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {

        if (sceneName != Define.Scene.GamePlayScene.ToString() && sceneName != Define.Scene.BattleScene.ToString())
            return;

        if (!clientsCompleted.Contains(Managers.RelayManager.NetworkManagerEx.LocalClientId))
            return;

        SetIndicatorOff();
    }

    public void OnDisable()
    {
        Managers.RelayManager.NetworkManagerEx.SceneManager.OnLoadEventCompleted -= OnChangeSceneEvent;
    }
    private Vector3 SetPosition()
    {
        Transform parentTr = GetComponentInParent<PlayerStats>().transform;
        Collider parentCollider = parentTr.GetComponent<Collider>();
        return  parentTr.position +  (Vector3.up * 1.5f);
    }

    protected override void StartInit()
    {
        transform.position = SetPosition();
        _indicatorIMG.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
       transform.rotation = Camera.main.transform.rotation;
    }

}
