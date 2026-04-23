using System.Collections.Generic;
using UnityEngine;

public static class ValueConstants
{
    public const int MaxRound = 5;      // 最大ラウンド数
    public const int MaxScore = 750;   // 最大スコア
    
    public const float InitialOneRoundTimeLimit = 15f;
    const float MaxScoreCoefficient = 1.75f;    // スコアの最大倍率
    const float MinScoreCoefficient = 0.1f;    // スコアの

    /// <summary>
    /// 総合スコアに基づいてスコアランクを計算するメソッド
    /// 割合計算でする。MaxScoreが変更されても対応できるようにするため。
    /// </summary>
    public static ScoreRank GetScoreRank(int totalScore)
    {
        // スコアの倍率も考慮する
        float scorePercentage = totalScore / (float)(MaxRound * MaxScore * MaxScoreCoefficient);   // 最大スコアに倍率を掛けた値で割って割合を計算

        if (scorePercentage >= 0.95f) return ScoreRank.S;
        else if (scorePercentage >= 0.9f) return ScoreRank.A;
        else if (scorePercentage >= 0.7f) return ScoreRank.B;
        else if (scorePercentage >= 0.8f) return ScoreRank.C;
        else return ScoreRank.D;
    }

    public static float GetScoreCoefficient(float difference)
    {
        return Mathf.Lerp(MaxScoreCoefficient, MinScoreCoefficient, difference);   // 差に応じて倍率を線形補間で計算
    }

    /// <summary>
    /// お題の色とPlayerが決定した色の差を元にスコアを計算するメソッド
    /// </summary>
    /// <param name="difference">お題の色とPlayerが決定した色の差（0.0〜1.0）</param>
    public static int GetScore(float difference)
    {
        float coefficient = GetScoreCoefficient(difference);
        int score = Mathf.RoundToInt(MaxScore * coefficient);
        return score;
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