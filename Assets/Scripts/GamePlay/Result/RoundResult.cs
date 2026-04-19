using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class RoundResult
{
    List<PreviewSlider> topic_preview_sliders = new List<PreviewSlider>();
    List<PreviewSlider> answer_preview_sliders = new List<PreviewSlider>();
    List<Label> difference_ranges = new List<Label>();
    Label roundCount;
    VisualElement topic_previewColor;
    VisualElement answer_previewColor;
    Label score;
    Button nextRoundButton;
    Material[] rgbMats;

    public RoundResult(VisualElement root, Material[] _rgbMats, ResultData resultData, System.Action onNextRound = null)
    {
        rgbMats = _rgbMats;
        VisualElement scoreRoot = root.Q<VisualElement>("round-result-root");
        if (scoreRoot == null) { Debug.LogError("RoundResultScoreのVisualElementが見つかりませんでした"); return; }

        roundCount = scoreRoot.Q<Label>("round-count");
        score = scoreRoot.Q<Label>("score-value");
        nextRoundButton = scoreRoot.Q<Button>();

        var topic_area = root.Q<VisualElement>("topic-area");
        var answer_area = root.Q<VisualElement>("answer-area");

        if(topic_area == null) { Debug.LogError("topic-areaのVisualElementが見つかりませんでした"); return; }
        if(answer_area == null) { Debug.LogError("answer-areaのVisualElementが見つかりませんでした"); return; }

        topic_previewColor = topic_area.Q<VisualElement>("preview-color");
        answer_previewColor = answer_area.Q<VisualElement>("preview-color");

        if (topic_previewColor == null) { Debug.LogError("お題のpreview-colorのVisualElementが見つかりませんでした"); return; }
        if (answer_previewColor == null) { Debug.LogError("プレイヤーのpreview-colorのVisualElementが見つかりませんでした"); return; }

        List<VisualElement> topic_preview_roots = topic_area.Query<VisualElement>("preview-root").ToList();
        List<VisualElement> answer_preview_roots = answer_area.Query<VisualElement>("preview-root").ToList();
        difference_ranges = root.Q<VisualElement>("difference-range").Q<VisualElement>("container").Query<Label>().ToList();

        if (roundCount == null) { Debug.LogError("round-countのLabelが見つかりませんでした"); return; }
        if (score == null) { Debug.LogError("score-valueのLabelが見つかりませんでした"); return; }
        if (nextRoundButton == null) { Debug.LogError("NextRoundButtonのButtonが見つかりませんでした"); return; }
        if (topic_preview_roots.Count != 3) { Debug.LogError($"{topic_preview_roots.Count} : お題のPreviewSliderの数が3つではありませんでした"); return; }
        if (answer_preview_roots.Count != 3) { Debug.LogError($"{answer_preview_roots.Count} : プレイヤーのPreviewSliderの数が3つではありませんでした"); return; }
        if (difference_ranges.Count != 3) { Debug.LogError($"{difference_ranges.Count} : 差分のLabelの数が3つではありませんでした"); return; }

        if(resultData.RoundCount >= GameManager.MaxRound) nextRoundButton.text = "結果発表";    // 最終ラウンドの場合はボタンのテキストを変更する
        nextRoundButton.RegisterCallback<ClickEvent>(evt => onNextRound.Invoke());     // 次のラウンドに進むボタンに処理を登録
        CreatePreviewSliders(topic_preview_roots, answer_preview_roots);        // お題の色とPlayerが決定した色のPreviewSliderを作成する
        AssignResultDataToUI(resultData);       // ResultDataの内容をUIに反映する
    }

    /// <summary>
    /// お題の色とPlayerが決定した色のPreviewSliderを作成する
    /// </summary>
    void CreatePreviewSliders(List<VisualElement> topic_preview_roots, List<VisualElement> answer_preview_roots)
    {
        for (int ii = 0; ii < topic_preview_roots.Count; ii++)
        {
            int index = ii;     // クロージャ対策
            PreviewSlider topic_preview_slider = new PreviewSlider(topic_preview_roots[index], (RGBType)index, rgbMats[index]);
            PreviewSlider answer_preview_slider = new PreviewSlider(answer_preview_roots[index], (RGBType)index, rgbMats[index]);
            topic_preview_sliders.Add(topic_preview_slider);
            answer_preview_sliders.Add(answer_preview_slider);
        }
    }

    /// <summary>
    /// ResultDataの内容をUIに反映するメソッド
    /// </summary>
    void AssignResultDataToUI(ResultData resultData)
    {
        roundCount.text = $"Round {resultData.RoundCount}";
        score.text = resultData.GetScore().ToString();
        topic_previewColor.style.backgroundColor = resultData.TopicColor;
        answer_previewColor.style.backgroundColor = resultData.AnswerColor;

        // お題の色とPlayerが決定した色の差をUIに反映する
        for (int ii = 0; ii < topic_preview_sliders.Count; ii++)
        {
            topic_preview_sliders[ii].AdjustChanged(resultData.TopicColor[(int)topic_preview_sliders[ii].Type]);
            answer_preview_sliders[ii].AdjustChanged(resultData.AnswerColor[(int)answer_preview_sliders[ii].Type]);
            difference_ranges[ii].text = topic_preview_sliders[ii].Type.ToString() + ": " + resultData.GetDifferenceByColor()[(int)topic_preview_sliders[ii].Type].ToString("F2");
        }
    }
}