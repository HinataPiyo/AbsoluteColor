using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public enum PanelMotionState
{ Hide, Show }

public static class CoreResource
{
#region ColorChannelData
    static ColorChannelData CurrentColorChannelData;    // 現在の色チャンネルデータを保持するプロパティ
    public static ColorChannelData GetColorChannelData()
    {
        if(CurrentColorChannelData == null)
        {
            var so = UnityEditor.AssetDatabase.LoadAssetAtPath<ColorChannelDataSO>("Assets/Datas/ColorChannelDataSO.asset");
            if(so == null)
            {
                Debug.LogError("Failed to load ColorChannelData from Assets/Resources/ColorChannelDataSO.asset");
            }
            return so.Datas[0];   // とりあえず最初のデータを返す
        }

        return CurrentColorChannelData;
    }

    public static void SetColorChannelData(ColorChannelData data)
    {
        CurrentColorChannelData = data;
    }
#endregion

    /// <summary>
    /// ジャンルごとのハイスコアを管理する辞書
    /// </summary>
    public static Dictionary<AdjustColorGenre, int> HighScores = new Dictionary<AdjustColorGenre, int>()
    {
        { AdjustColorGenre.RGB_Slider, 0 }
    };

    /// <summary>
    /// スコア履歴のデータを管理するスタック
    /// </summary>
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

    /// <summary>
    /// UIパネルの表示状態を変更するメソッド
    /// </summary>
    /// <param name="targetPanel"></param>
    /// <param name="state"></param>
    public static void ChangePanelMotion(VisualElement targetPanel, PanelMotionState state)
    {
        targetPanel.ClearClassList();
        switch (state)
        {
            case 
                PanelMotionState.Hide:
                targetPanel.AddToClassList("hide-panel");
                break;
            case PanelMotionState.Show:
                targetPanel.AddToClassList("show-panel");
                break;
        }
    }

}