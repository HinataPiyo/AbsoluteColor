using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class VariousButtonsController : MonoBehaviour, IUIModuleController
{
    public SelectModeButton SelectModeButton { get; private set; }
    Button playButton;
    Button collectionButton;
    Button eventButton;
    Button rankingButton;
    Button scoreHistoryButton;

    public void Initialize(VisualElement root, Material[] Mats = null)
    {
        SelectModeButton = new SelectModeButton(root, CoreResource.CurrentColorChannelData);   // 最初のモードのデータを渡してSelectModeButtonを初期化
        playButton = root.Q<Button>("button-play");
        collectionButton = root.Q<Button>("button-collection");
        eventButton = root.Q<Button>("button-event");
        rankingButton = root.Q<Button>("button-ranking");
        scoreHistoryButton = root.Q<Button>("button-score-history");

        playButton.clicked += PlayButtonOnClick;
    }

    /// <summary>
    /// PlayButtonを押したらゲームプレイシーンに遷移する
    /// </summary>
    void PlayButtonOnClick()
    {
        SceneManager.LoadScene(NameConstant.GamePlayScene);
    }

    /// <summary>
    /// ScoreHistoryButtonを押したらスコア履歴パネルを表示する
    /// </summary>
    public void ScoreHistoryButtonOnClick(System.Action action)
    {
        scoreHistoryButton.clicked += () => {
            action();
            AudioManager.I.PlaySE("NormalButton");
        };
    }
}

public class SelectModeButton
{
    Button selectModeButton;
    Label modeText;
    Label hightScoreText;

    public SelectModeButton(VisualElement root, ColorChannelData data)
    {
        selectModeButton = root.Q<Button>("button-select-mode");
        modeText = root.Q<Label>("selected-mode-text");
        hightScoreText = root.Q<Label>("high-score-value");

        modeText.text = $"モード: {data.ModeName}";                 // モード名を表示
        hightScoreText.text = CoreResource.HighScores[data.AdjustColorGenre].ToString("#,###");     // ハイスコアを表示
    }

    /// <summary>
    /// SelectModeButtonを押したらモード選択パネルを表示する
    /// </summary>
    public void ButtonOnClick(System.Action action, ColorChannelData data)
    {
        selectModeButton.clicked += () =>
        {
            modeText.text = $"モード: {data.ModeName}";    // モード名を表示
            hightScoreText.text = CoreResource.HighScores[data.AdjustColorGenre].ToString("#,###");    // ハイスコアを表示
            action();
            AudioManager.I.PlaySE("NormalButton");
        };
    }
}