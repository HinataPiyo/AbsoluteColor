using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class AdjustRGBSlidedr : PanelChangeElementBase
{
    VisualElement adjust_RGB_Panel;    // RGB調整用のパネル
    
    List<AdjustSlider> adjust_sliders = new List<AdjustSlider>();
    List<PreviewSlider> preview_sliders = new List<PreviewSlider>();

    float r, g, b;
    
    int rSeStep, gSeStep, bSeStep;


    public AdjustRGBSlidedr(VisualElement root)
    {
        Root = root;
        adjust_RGB_Panel = root.Q<VisualElement>("rgb-adjust-color-panel");   // RGB調整用のパネルを取得する
        List<VisualElement> bars = adjust_RGB_Panel.Q<VisualElement>("adjust-color-sliders").Query<VisualElement>("adjust-slider-root").ToList();

        for (int ii = 0; ii < bars.Count; ii++)
        {
            int index = ii;     // クロージャ対策
            AdjustSlider adjust_slider = new AdjustSlider(bars[index], (RGBType)index, OnSliderValueChanged);
            PreviewSlider preview_slider = new PreviewSlider(bars[index], (RGBType)index, HomeManager.I.ColorChannelDataSO.GetColorChannelData(AdjustColorGenre.RGB_Slider).Materials[index]);
            adjust_sliders.Add(adjust_slider);
            preview_sliders.Add(preview_slider);
            OnSliderValueChanged((RGBType)index, 0f);     // スライダーの初期値を0にする
        }
    }

    /// <summary>
    /// スライダーの値が変更されたときの処理
    /// </summary>
    /// <param name="type">RGBのどれか</param>
    /// <param name="value">スライダーの値</param>
    public void OnSliderValueChanged(RGBType type, float value)
    {
        bool shouldPlaySe = false;

        switch (type)
        {
            case RGBType.R:
                r = value;
                shouldPlaySe = HasSecondDecimalChanged(ref rSeStep, value);
                break;
            case RGBType.G:
                g = value;
                shouldPlaySe = HasSecondDecimalChanged(ref gSeStep, value);
                break;
            case RGBType.B:
                b = value;
                shouldPlaySe = HasSecondDecimalChanged(ref bSeStep, value);
                break;
        }

        preview_sliders.Find(slider => slider.Type == type).AdjustChanged(value);       // プレビュー用のスライダーも値を変更する
        HomeManager.I.UpdatePreviewColor(new Color(r, g, b));               // プレビュー用の色を変更する
        if (shouldPlaySe) AudioManager.I.PlaySE("MoveAdjustSlider");        // 小数第2位以上が変化したときのみSEを再生する
    }

    /// <summary>
    /// 小数第2位以上が変化したかどうかを判定するためのメソッド
    /// </summary>
    /// <param name="previousStep">前回の小数第2位までの値を100倍して整数にしたもの</param>
    /// <param name="value">スライダーの値</param>
    bool HasSecondDecimalChanged(ref int previousStep, float value)
    {
        int currentStep = Mathf.FloorToInt(value * 100f);
        bool hasChanged = currentStep != previousStep;
        previousStep = currentStep;
        return hasChanged;
    }
}