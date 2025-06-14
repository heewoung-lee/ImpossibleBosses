using TMPro;
using UI.Scene;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class UI_Loading : UIScene
{
    enum Texts
    {
        Text_LoadingValue
    }
    enum Sliders
    {
        Slider_LoadingBar
    }



    TMP_Text _text_LoadingValue;
    Slider _loadingSlider;

    public float LoaingSliderValue
    {
        get
        {
            return _loadingSlider.value;
        }

        set
        {
            _loadingSlider.value = value;
            _text_LoadingValue.text = $"Loading...{(int)(_loadingSlider.value * 100f)}";
        }
    }


    protected override void StartInit()
    {
        base.StartInit();
        SetSortingOrder((int)Define.SpecialSortingOrder.LoadingScreen);
    }

    protected override void AwakeInit()
    {
        base.AwakeInit();
        Bind<TMP_Text>(typeof(Texts));
        Bind<Slider>(typeof(Sliders));
        _text_LoadingValue = Get<TMP_Text>((int)Texts.Text_LoadingValue);
        _loadingSlider = Get<Slider>((int)Sliders.Slider_LoadingBar);

        _loadingSlider.value = 0f;
    }


}
