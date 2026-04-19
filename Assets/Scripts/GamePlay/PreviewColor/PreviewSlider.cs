using UnityEngine;
using UnityEngine.UIElements;
using System;

[Serializable]
public class PreviewSlider
{
    Slider preview_slider;
    Label parameter;
    public RGBType Type { get; private set; }

    public PreviewSlider(VisualElement root, RGBType _type, Material _mat)
    {
        Type = _type;
        VisualElement preview_slider_root = root.Q<VisualElement>("preview-root");
        if (preview_slider_root == null) { Debug.LogError("PreviewColorSliderのVisualElementが見つかりませんでした"); return; }

        preview_slider = preview_slider_root.Q<Slider>();
        if (preview_slider == null) { Debug.LogError("PreviewColorSliderのSliderが見つかりませんでした"); return; }

        parameter = preview_slider_root.Q<Label>("parameter");
        if (parameter == null) { Debug.LogError("SliderのLabelが見つかりませんでした"); return; }

        var preview_track = preview_slider.Q<VisualElement>(className: "unity-base-slider__tracker");
        var preview_container = preview_slider.Q<VisualElement>(className: "unity-base-slider__drag-container");
        var preview_dragger_border = preview_slider.Q<VisualElement>(className: "unity-base-slider__dragger-border");
        var preview_dragger = preview_slider.Q<VisualElement>(className: "unity-base-slider__dragger");

        if (preview_dragger == null) { Debug.LogError("Sliderのdraggerが見つかりませんでした"); return; }
        if (preview_dragger_border == null) { Debug.LogError("Sliderのdragger-borderが見つかりませんでした"); return; }
        if (preview_track == null) { Debug.LogError("Sliderのtrackerが見つかりませんでした"); return; }
        if (preview_container == null) { Debug.LogError("Sliderのdrag-containerが見つかりませんでした"); return; }
        if (_mat == null) { Debug.LogError("Materialがnullでした"); return; }

        // スライダーの背景色を設定
        preview_track.style.unityMaterial = new StyleMaterialDefinition(_mat);
        preview_track.pickingMode = PickingMode.Ignore;
        preview_container.pickingMode = PickingMode.Ignore;
        preview_dragger_border.pickingMode = PickingMode.Ignore;
        preview_dragger.pickingMode = PickingMode.Ignore;

        // スライダーの初期値と範囲を設定
        preview_slider.value = 0f;
        preview_slider.lowValue = 0f;
        preview_slider.highValue = 1f;
        preview_slider.label = _type.ToString();

        parameter.text = preview_slider.value.ToString("F2");
    }

    public void AdjustChanged(float value)
    {
        preview_slider.value = value;
        parameter.text = preview_slider.value.ToString("F2");
    }

}