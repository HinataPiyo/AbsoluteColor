using System;
using UnityEngine;
using UnityEngine.UIElements;

public partial class ScoreHistoryData
{
    public ColorChannelData ColorChannelData { get; private set; }
    public ScoreRank ScoreRank { get; private set; }
    public int Score { get; private set; }
    public DateTime endTime { get; private set; }       // スコア履歴の終了時間

    public ScoreHistoryData(ColorChannelData colorChannelData, ScoreRank scoreRank, int score, DateTime endTime)
    {
        ColorChannelData = colorChannelData;
        ScoreRank = scoreRank;
        Score = score;
        this.endTime = endTime;
    }
}

/// <summary>
/// スコア履歴のUI構成を管理するクラス
/// </summary>
public class ScoreHistoryComposition
{
    Button button;
    Label modeText;
    Label timeText;
    Label rankText;
    Label scoreText;

    public ScoreHistoryComposition(VisualElement root, ScoreHistoryData data, Action onClick)
    {
        modeText = root.Q<Label>("mode-value");
        timeText = root.Q<Label>("time-value");
        rankText = root.Q<Label>("rank-value");
        scoreText = root.Q<Label>("score-value");

        modeText.text = $"モード: {data.ColorChannelData.ModeName}";
        timeText.text = data.endTime.ToString("MM/dd HH:mm");
        rankText.text = data.ScoreRank.ToString();
        scoreText.text = data.Score.ToString("#,###");

        button = root.Q<Button>();
        onClick += () => button.clicked += onClick;
    }
}