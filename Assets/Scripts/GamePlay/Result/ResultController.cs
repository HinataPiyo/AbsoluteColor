using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public partial class ResultData
{
    public Color TopicColor { get; private set; }    // お題の色
    public Color AnswerColor { get; private set; }   // Playerが決定した色
    public int RoundCount { get; private set; }    // ラウンド数

    public ResultData(Color _topicColor, Color _answerColor, int _roundCount)
    {
        TopicColor = _topicColor;
        AnswerColor = _answerColor;
        RoundCount = _roundCount;
    }

    /// <summary>
    /// お題の色とPlayerが決定した色の差を計算するメソッド
    /// </summary>
    public Vector3 GetDifferenceByColor()
    {
        // お題の色とPlayerが決定した色の差を計算する（ここでは単純にRGBの差の絶対値を使用）
        // UI表示用としてマイナス表記も欲しい
        float rDiff = AnswerColor.r - TopicColor.r;
        float gDiff = AnswerColor.g - TopicColor.g;
        float bDiff = AnswerColor.b - TopicColor.b;
        return new Vector3(rDiff, gDiff, bDiff);
    }

    /// <summary>
    /// お題の色とPlayerが決定した色の差を計算するメソッド（値で返す）
    /// </summary>
    public float GetDifferenceByValue()
    {
        // お題の色とPlayerが決定した色の差を計算する（ここでは単純にRGBの差の絶対値の平均を使用）
        float rDiff = Mathf.Abs(TopicColor.r - AnswerColor.r);
        float gDiff = Mathf.Abs(TopicColor.g - AnswerColor.g);
        float bDiff = Mathf.Abs(TopicColor.b - AnswerColor.b);
        return (rDiff + gDiff + bDiff) / 3f;
    }

    /// <summary>
    /// お題の色とPlayerが決定した色の差を元にスコアを計算するメソッド
    /// </summary>
    public int GetScore()
    {
        // 差が小さいほどスコアが高くなるように計算する（ここでは単純に差の逆数を使用）
        float difference = GetDifferenceByValue();
        return Mathf.RoundToInt((1f - difference) * 100);   // スコアは0から100の範囲で返す
    }
}

public class ResultController : MonoBehaviour, IUIModuleController
{
    public readonly List<ResultData> resultColors = new List<ResultData>();    // お題の色とPlayerが決定した色のリスト

    VisualElement resultRoot;
    public bool IsResultDisplayed { get; private set; } = false;    // 結果が表示されているかどうかのフラグ
    Material[] colorMaterials;


    public void Initialize(VisualElement root, Material[] Mats)
    {
        colorMaterials = Mats;
        resultRoot = root;
        resultRoot.style.display = DisplayStyle.None;   // 最初は結果表示用のUIを非表示にする
    }

    /// <summary>
    /// 結果の色を追加するメソッド
    /// </summary>
    public void AddResultColor(Color topicColor, Color playerColor)
    {
        resultRoot.style.display = DisplayStyle.Flex;       // 結果表示用のUIを表示する
        resultColors.Add(new ResultData(topicColor, playerColor, resultColors.Count + 1));

        // 結果の色をUIに反映する
        new RoundResult(resultRoot, colorMaterials, resultColors[resultColors.Count - 1], () => {
            IsResultDisplayed = false;      // 結果が表示されていない状態にする
            resultRoot.style.display = DisplayStyle.None;   // 結果表示用のUIを非表示にする
        });

        IsResultDisplayed = true;    // 結果が表示されているフラグを立てる
    }
}