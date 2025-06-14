using System.Collections.Generic;
using GameManagers;
using Stats;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Util;

namespace UI.WorldSpace
{
    public class UIPortalIndicator : UIBase
    {
        enum Images
        {
            PortalIndicatorImg
        }


        private Image _indicatorImg;


        public void SetIndicatorOff()
        {
            _indicatorImg.gameObject.SetActive(false);
        }
        public void SetIndicatorOn()
        {
            _indicatorImg.gameObject.SetActive(true);
        }

        protected override void AwakeInit()
        {
            Bind <Image>(typeof(Images));
            _indicatorImg = Get<Image>((int)Images.PortalIndicatorImg);

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
            _indicatorImg.gameObject.SetActive(false);
        }

        void LateUpdate()
        {
            transform.rotation = Camera.main.transform.rotation;
        }

    }
}
