using TMPro;
using UnityEngine.UI;
using Util;

namespace UI.Scene.SceneUI
{
    public class UILoading : UIScene
    {
        enum Texts
        {
            TextLoadingValue
        }
        enum Sliders
        {
            SliderLoadingBar
        }
        TMP_Text _textLoadingValue;
        Slider _loadingSlider;

        public float LoaingSliderValue
        {
            get => _loadingSlider.value;
            set
            {
                _loadingSlider.value = value;
                _textLoadingValue.text = $"Loading...{(int)(_loadingSlider.value * 100f)}";
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
            _textLoadingValue = Get<TMP_Text>((int)Texts.TextLoadingValue);
            _loadingSlider = Get<Slider>((int)Sliders.SliderLoadingBar);

            _loadingSlider.value = 0f;
        }


    }
}
