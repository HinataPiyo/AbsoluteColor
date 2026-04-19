using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AdjustUIAreaController: MonoBehaviour, IUIModuleController
{
    List<AdjustSlider> adjust_sliders = new List<AdjustSlider>();
    List<PreviewSlider> preview_sliders = new List<PreviewSlider>();
    VisualElement previewColor;
    Button decideButton;
    float r, g, b;
    int rSeStep, gSeStep, bSeStep;

    Material[] rgbMats;

    public void Initialize(VisualElement root, Material[] Mats)
    {
        rgbMats = Mats;

        // 色調整を行うProgressbarを探す
        List<VisualElement> bars = root.Q<VisualElement>("adjust-color-sliders").Query<VisualElement>("adjust-slider-root").ToList();
        previewColor = root.Q<VisualElement>("preview-color");
        decideButton = root.Q<Button>("decide-button");

        if (bars.Count != 3) { Debug.LogError($"{bars.Count} : Progressbarの数が3つではありませんでした"); return; }
        if (rgbMats == null || rgbMats.Length != 3) { Debug.LogError("Materialの数が3つではありませんでした"); return; }
        if (previewColor == null) { Debug.LogError("プレビュー用のVisualElementが見つかりませんでした"); return; }
        if (decideButton == null) { Debug.LogError("決定用のButtonが見つかりませんでした"); return; }

        // Progressbarを動的に処理を実行するための初期化
        for (int ii = 0; ii < bars.Count; ii++)
        {
            int index = ii;     // クロージャ対策
            AdjustSlider adjust_slider = new AdjustSlider(bars[index], (RGBType)index, OnSliderValueChanged);
            PreviewSlider preview_slider = new PreviewSlider(bars[index], (RGBType)index, rgbMats[index]);
            adjust_sliders.Add(adjust_slider);
            preview_sliders.Add(preview_slider);
            OnSliderValueChanged((RGBType)index, 0f);     // スライダーの初期値を0にする
        }

        // 決定用のButtonに処理を登録
        decideButton.RegisterCallback<ClickEvent>(evt => {
            GameManager.I.isPlayerDecided = true;
            AudioManager.I.PlaySE("DecideAdjustColorButton");
        });
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
        UpdatePreviewColor(new Color(r, g, b));         // プレビュー用のVisualElementの背景色を変更する
        if (shouldPlaySe) AudioManager.I.PlaySE("MoveAdjustSlider");      // 小数第2位以上が変化したときのみSEを再生する
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

    /// <summary>
    /// プレビュー用のVisualElementの背景色を更新する
    /// </summary>
    public void UpdatePreviewColor(Color color)
    {
        previewColor.style.backgroundColor = new StyleColor(color);
    }

    public Color GetDecidedColor()
    {
        return new Color(r, g, b);
    }
}