using System.Collections.Generic;
using UnityEngine;

public static class ValueConstants
{
    public const int MaxRound = 5;      // 最大ラウンド数
    public const int MaxScore = 1000;   // 最大スコア
    
    public const float InitialOneRoundTimeLimit = 15f;

    /// <summary>
    /// 総合スコアに基づいてスコアランクを計算するメソッド
    /// 割合計算でする。MaxScoreが変更されても対応できるようにするため。
    /// </summary>
    public static ScoreRank GetScoreRank(int totalScore)
    {
        float scorePercentage = (float)totalScore / (MaxRound * MaxScore);   // 総合スコアの割合を計算

        if (scorePercentage >= 0.95f) return ScoreRank.S;
        else if (scorePercentage >= 0.9f) return ScoreRank.A;
        else if (scorePercentage >= 0.7f) return ScoreRank.B;
        else if (scorePercentage >= 0.8f) return ScoreRank.C;
        else return ScoreRank.D;
    }

    /// <summary>
    /// 総合スコアに基づいてスコアランクを計算するメソッド
    /// </summary>
    public static readonly Dictionary<ScoreRank, Color> ScoreRankColors = new Dictionary<ScoreRank, Color>()
    {
        { ScoreRank.S, new Color(1f, 0.84f, 0f) },   // ゴールド
        { ScoreRank.A, new Color(0.75f, 0.75f, 0.75f) },   // シルバー
        { ScoreRank.B, new Color(0.8f, 0.5f, 0.2f) },   // ブロンズ
        { ScoreRank.C, new Color(0.5f, 0.5f, 0.5f) },   // グレー
        { ScoreRank.D, new Color(1f, 0f, 0f) }    // レッド
    };
}

public enum ScoreRank { S, A, B, C, D }