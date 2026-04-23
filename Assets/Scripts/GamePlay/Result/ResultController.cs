using System.Collections.Generic;
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
        // 差が小さいほど倍率が高くなり、差が大きいほど倍率が低くなる
        float maxCoefficient = 1.75f;    // スコアの最大倍率
        float minCoefficient = 0.1f;    // スコアの最小倍率
        float coefficient = Mathf.Lerp(maxCoefficient, minCoefficient, GetDifferenceByValue());   // 差に応じて倍率を線形補間で計算
        int score = Mathf.RoundToInt(ValueConstants.MaxScore * coefficient);   // 最大スコアに倍率を掛けて最終的なスコアを計算
        return score;
    }

    /// <summary>
    /// お題の色とPlayerが決定した色の差を元にジャッジメントの種類を返すメソッド
    /// </summary>
    public JugementType GetJugementType()
    {
        float difference = GetDifferenceByValue();
        if (difference < 0.05f) return JugementType.Perfect;
        else if (difference < 0.15f) return JugementType.Excellent;
        else if (difference < 0.3f) return JugementType.Great;
        else if (difference < 0.5f) return JugementType.Good;
        else return JugementType.Bad;
    }

    public float GetParcentage()
    {
        return (1f - GetDifferenceByValue()) * 100f;   // パーセンテージは0%から100%の範囲で返す
    }
}

public class ResultController : MonoBehaviour, IUIModuleController
{
    public readonly List<ResultData> resultColors = new List<ResultData>();    // お題の色とPlayerが決定した色のリスト

    VisualElement resultRoot;
    public bool IsResultDisplayed { get; private set; } = false;    // 結果が表示されているかどうかのフラグ
    Material[] colorMaterials;


    public void Initialize(VisualElement root)
    {
        colorMaterials = CoreResource.GetColorChannelData().Materials;
        resultRoot = root;
        CoreResource.ChangePanelMotion(resultRoot, PanelMotionState.Hide);    // 結果表示用のUIを初期状態では非表示にする
    }

    /// <summary>
    /// 結果の色を追加するメソッド
    /// </summary>
    public ResultData CreateResultData(Color topicColor, Color playerColor)
    {
        ResultData data = new ResultData(topicColor, playerColor, resultColors.Count + 1);
        resultColors.Add(data);

        IsResultDisplayed = true;    // 結果が表示されているフラグを立てる
        return data;
    }

    /// <summary>
    /// 結果を表示するメソッド
    /// </summary>
    public void ShowResult()
    {
        resultRoot.style.display = DisplayStyle.Flex;       // 結果表示用のUIを表示する
        CoreResource.ChangePanelMotion(resultRoot, PanelMotionState.Show);    // 結果表示用のUIを開くアニメーションを再生する

        // 結果の色をUIに反映する
        new RoundResult(resultRoot, colorMaterials, resultColors[resultColors.Count - 1], () => {
            CoreResource.ChangePanelMotion(resultRoot, PanelMotionState.Hide);    // 結果表示用のUIを初期状態では非表示にする
            resultRoot.style.display = DisplayStyle.None;       // 結果表示用のUIを非表示にする
            IsResultDisplayed = false;      // 結果が表示されていない状態にする
        });
    }
    


    public void GetJugement(ResultData data, out JugementType type, out float parcentage)
    {
        if(data == null) { Debug.LogError("ResultData is null"); type = JugementType.Bad; parcentage = 0f; return; }

        type = data.GetJugementType();
        parcentage = data.GetParcentage();
    }
}