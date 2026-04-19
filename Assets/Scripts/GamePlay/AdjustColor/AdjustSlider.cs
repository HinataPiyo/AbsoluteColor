using System;
using UnityEngine;
using UnityEngine.UIElements;

public enum RGBType { R, G, B }

[Serializable]
public partial class AdjustSlider
{
    Slider adjust_slider;

    /// <summary>
    /// 1. 色調整を行うSlider
    /// 2. Sliderの背景色をRGBのどれかにする(背景色は実際のグラデーションにする)
    /// </summary>
    public AdjustSlider(VisualElement root, RGBType _type, Action<RGBType, float> onValueChanged)
    {
        adjust_slider = root.Q<Slider>("adjust-slider");

        if (adjust_slider == null) { Debug.LogError("adjust-sliderのSliderが見つかりませんでした"); return; }

        if (onValueChanged == null) { Debug.LogError("onValueChangedがnullでした"); return; }

        adjust_slider.value = 0f;
        adjust_slider.lowValue = 0f;
        adjust_slider.highValue = 1f;

        // スライダーの値が変更されたときの処理を登録
        adjust_slider.RegisterValueChangedCallback(evt => onValueChanged.Invoke(_type, adjust_slider.value));      // adjust_sliderの値が変更されたときにpreview_sliderの値も変更する
    }

}