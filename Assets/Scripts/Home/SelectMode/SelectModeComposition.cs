using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// モード選択のUIを構成するクラス
/// </summary>
public class SelectModeComposition
{
    Button modeButton;
    Label modeText;
    Label modeDescription;
    Label highScore;
    VisualElement now_mode_container;

    public SelectModeComposition(VisualElement root, ColorChannelData data, System.Action<SelectModeComposition> onClickAction)
    {
        modeText = root.Q<Label>("selected-mode-value");
        modeDescription = root.Q<Label>("description-value");
        highScore = root.Q<Label>("high-score-value");
        now_mode_container = root.Q<VisualElement>("now-mode-container");
        modeButton = root.Q<Button>("mode-composition-root");

        modeButton.clicked += () => {
            CoreResource.CurrentColorChannelData = data;    // 選択されたモードのデータをCoreResourceに保存する
            SetNowModeTagActive(true);        // 現在選択中のタグを表示する
            onClickAction(this);
        };

        SetNowModeTagActive(false);       // 現在選択中のタグは初期状態では非表示にする

        modeText.text = $"モード: {data.ModeName}";             // モードの名前を表示
        modeDescription.text = data.Description;                // モードの説明文を表示
        highScore.text = CoreResource.HighScores[data.AdjustColorGenre].ToString("#,###");        // ハイスコアを表示
        
    }

    public void SetNowModeTagActive(bool isActive)
    {
        now_mode_container.style.display = isActive ? DisplayStyle.Flex : DisplayStyle.None;   // 現在選択中のタグの表示・非表示を切り替える
    }
}