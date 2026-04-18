using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum ScoreRank { S, A, B, C, D }
public class AllRoundTotalResult : MonoBehaviour
{
    [SerializeField] VisualTreeAsset roundResultTemplate;    // ラウンドごとの結果を表示するためのUIテンプレート
    [SerializeField] Material[] rgbMats;    // スコアランクごとの色を設定するためのMaterial
    UIDocument uiDoc;
    ScrollView resultScrollView;    // 結果を表示するためのScrollView
    Label totalScore;           // 総合スコアを表示するためのLabel
    Label scoreRank;            // スコアランクを表示するためのLabel
    Button backHomeButton;      // ホームに戻るためのButton
    Button retryButton;         // もう一度プレイするためのButton

    VisualElement allroundResultRoot;    // 全ラウンドの結果を表示するためのVisualElement

    void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
    }

    void OnEnable()
    {
        allroundResultRoot = uiDoc.rootVisualElement.Q<VisualElement>("AllRoundResultScore");
        if (allroundResultRoot == null) { Debug.LogError("allroundResultRootのVisualElementが見つかりませんでした"); return; }
        
        resultScrollView = allroundResultRoot.Q<ScrollView>();
        totalScore = allroundResultRoot.Q<Label>("total-score-value");
        scoreRank = allroundResultRoot.Q<Label>("score-rank-value");
        backHomeButton = allroundResultRoot.Q<Button>("button-back-home");
        retryButton = allroundResultRoot.Q<Button>("button-retry");

        if (resultScrollView == null) { Debug.LogError("resultScrollViewが見つかりませんでした"); return; }
        if (totalScore == null) { Debug.LogError("totalScoreが見つかりませんでした"); return; }
        if (scoreRank == null) { Debug.LogError("scoreRankが見つかりませんでした"); return; }
        if (backHomeButton == null) { Debug.LogError("backHomeButtonが見つかりませんでした"); return; }
        if (retryButton == null) { Debug.LogError("retryButtonが見つかりませんでした"); return; }

        resultScrollView.Clear();   // ScrollViewの内容をクリアする
        allroundResultRoot.style.display = DisplayStyle.None;   // 最初は全ラウンドの結果表示用のUIを非表示にする
    }

    /// <summary>
    /// 全ラウンドの結果を表示するためのメソッド
    /// </summary>
    /// <param name="resultDataList">お題の色とPlayerが決定した色のリスト</param>
    public void ShowAllRoundResult(List<ResultData> resultDataList)
    {
        allroundResultRoot.style.display = DisplayStyle.Flex;   // 全ラウンドの結果表示用のUIを表示する

        // ラウンドごとの結果をScrollViewに追加する
        for (int ii = 0; ii < resultDataList.Count; ii++)
        {
            ResultData data = resultDataList[ii];
            VisualElement elem = roundResultTemplate.Instantiate();   // テンプレートからUIを生成する
            new RoundResult(elem, rgbMats, data);    // 生成したUIにラウンドの結果を表示するための処理を行う
            resultScrollView.Add(elem);   // ScrollViewに生成したUIを追加する
        }
            
        // 総合スコアとランクの計算と表示
        int totalScoreValue = CalculateTotalScore(resultDataList);
        totalScore.text = totalScoreValue.ToString();

        ScoreRank rank = CalculateScoreRank(totalScoreValue);
        scoreRank.text = rank.ToString();
        scoreRank.style.unityTextOutlineColor = scoreRankColors[rank];
    }

    /// <summary>
    /// 総合スコアを計算するメソッド
    /// </summary>
    int CalculateTotalScore(List<ResultData> resultDataList)
    {
        int totalScore = 0;
        foreach (var resultData in resultDataList)
        {
            totalScore += resultData.GetScore();
        }

        return totalScore;
    }

    /// <summary>
    /// 総合スコアに基づいてスコアランクを計算するメソッド
    /// </summary>
    ScoreRank CalculateScoreRank(int totalScore)
    {
        foreach (var kvp in scoreRankThresholds)
        {
            if (totalScore >= kvp.Value)
            {
                return kvp.Key;
            }
        }

        return ScoreRank.D;   // どのランクにも該当しない場合はDランクとする
    }

    /// <summary>
    /// 総合スコアに基づいてスコアランクを計算するメソッド
    /// </summary>
    readonly Dictionary<ScoreRank, int> scoreRankThresholds = new Dictionary<ScoreRank, int>()
    {
        { ScoreRank.S, 450 },
        { ScoreRank.A, 400 },
        { ScoreRank.B, 300 },
        { ScoreRank.C, 200 },
        { ScoreRank.D, 0 }
    };

    /// <summary>
    /// 総合スコアに基づいてスコアランクを計算するメソッド
    /// </summary>
    readonly Dictionary<ScoreRank, Color> scoreRankColors = new Dictionary<ScoreRank, Color>()
    {
        { ScoreRank.S, new Color(1f, 0.84f, 0f) },   // ゴールド
        { ScoreRank.A, new Color(0.75f, 0.75f, 0.75f) },   // シルバー
        { ScoreRank.B, new Color(0.8f, 0.5f, 0.2f) },   // ブロンズ
        { ScoreRank.C, new Color(0.5f, 0.5f, 0.5f) },   // グレー
        { ScoreRank.D, new Color(1f, 0f, 0f) }    // レッド
    };

}