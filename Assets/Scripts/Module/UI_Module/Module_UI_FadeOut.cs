using System.Collections;
using TMPro;
using Unity.Android.Types;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Module_UI_FadeOut : MonoBehaviour
{
    Image[] _ui_Images;
    TMP_Text[] _tmp_texts;

    private void Awake()
    {
        _ui_Images = GetComponentsInChildren<Image>();
        _tmp_texts = GetComponentsInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        StartCoroutine(FadeOutImage());
    }
    private void OnDisable()
    {
        foreach (Image image in _ui_Images)
        {
            image.color = image.color.WithAlpha(1f);
        }
        foreach (TMP_Text text in _tmp_texts)
        {
            text.color = text.color.WithAlpha(1f);
        }
    }
    IEnumerator FadeOutImage()
    {
        float duration = 1f;
        while(duration > 0)
        {
            duration -= Time.deltaTime/2f;
            foreach(Image image in _ui_Images)
            {
                image.color = image.color.WithAlpha(duration);
            }
            foreach(TMP_Text text in _tmp_texts)
            {
                text.color = text.color.WithAlpha(duration);
            }

            yield return null;
        }
        gameObject.SetActive(false);
    }

}
