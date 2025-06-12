using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Module.UI_Module
{
    public class ModuleUIFadeOut : MonoBehaviour
    {
        private Graphic[] _graphics;
        private bool _isPlayingFadeout = false;
        public bool IsPlayingFadeOut => _isPlayingFadeout;

        public Action DoneFadeoutEvent;

        private void Awake()
        {
            _graphics = GetComponentsInChildren<Graphic>();
        }

        private void OnEnable()
        {
            StartCoroutine(FadeOutImage());
        }

        private void OnDisable()
        {
            _isPlayingFadeout = false;
            DoneFadeoutEvent?.Invoke();
            // Disable될 때 컬러를 알파1로 초기화
            foreach (Graphic g in _graphics)
            {
                Color c = g.color;
                c.a = 1f;
                g.color = c;
            }
        }

        IEnumerator FadeOutImage()
        {
            _isPlayingFadeout = true;
            float duration = 1f;
            while (duration > 0)
            {
                duration -= Time.deltaTime / 2f;

                foreach (Graphic g in _graphics)
                {
                    Color c = g.color;
                    c.a = duration;
                    g.color = c;
                }

                yield return null;
            }
            gameObject.SetActive(false);
        }
    }
}
