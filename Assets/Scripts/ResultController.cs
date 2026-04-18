using System.Collections.Generic;
using UnityEngine;

public class ResultController : MonoBehaviour
{
    // 結果の色を保存する（お題とPlayerが決定した色）
    Dictionary<Color, Color> resultColors = new Dictionary<Color, Color>(); 

    /// <summary>
    /// 結果の色を追加するメソッド
    /// </summary>
    public void AddResultColor(Color topicColor, Color playerColor)
    {
        resultColors.Add(topicColor, playerColor);
        Debug.Log($"結果の色を追加しました。お題の色: {topicColor}, Playerの色: {playerColor}");
    }
}