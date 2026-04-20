using System.Collections.Generic;

public static class CoreResource
{
    public static ColorChannelData CurrentColorChannelData { get; set; }   // 現在の色チャンネルデータを保持するプロパティ
    public static Dictionary<AdjustColorGenre, int> HighScores = new Dictionary<AdjustColorGenre, int>()
    {
        { AdjustColorGenre.RGB_Slider, 0 }
    };

    public static Stack<ScoreHistoryData> ScoreHistoryDatas = new Stack<ScoreHistoryData>();   // スコア履歴のデータを管理するリスト

    /// <summary>
    /// ジャンルごとのハイスコアを更新するメソッド
    /// </summary>
    public static void UpdateHighScore(AdjustColorGenre genre, int score)
    {
        if (HighScores.ContainsKey(genre))
        {
            if (score > HighScores[genre])
            {
                HighScores[genre] = score;
            }
        }
        else
        {
            HighScores[genre] = score;
        }
    }
}