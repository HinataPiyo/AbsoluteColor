using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class AllRoundTotalResult : MonoBehaviour, IUIModuleController
{
    [SerializeField] VisualTreeAsset roundResultTemplate;    // ラウンドごとの結果を表示するためのUIテンプレート
    Material[] colorMaterials;    // スコアランクごとの色を設定するためのMaterial
    ScrollView resultScrollView;    // 結果を表示するためのScrollView
    Label totalScore;           // 総合スコアを表示するためのLabel
    Label scoreRank;            // スコアランクを表示するためのLabel
    Label scoreRankText;
    Button backHomeButton;      // ホームに戻るためのButton
    Button retryButton;         // もう一度プレイするためのButton

    VisualElement allroundResultRoot;    // 全ラウンドの結果を表示するためのVisualElement

    public void Initialize(VisualElement root)
    {
        colorMaterials = CoreResource.GetColorChannelData().Materials;
        allroundResultRoot = root;

        if (allroundResultRoot == null) { Debug.LogError("allroundResultRootのVisualElementが見つかりませんでした"); return; }
        
        resultScrollView = allroundResultRoot.Q<ScrollView>();
        totalScore = allroundResultRoot.Q<Label>("total-score-value");
        scoreRank = allroundResultRoot.Q<Label>("score-rank-value");
        scoreRankText = allroundResultRoot.Q<Label>("score-rank-text");
        backHomeButton = allroundResultRoot.Q<Button>("button-back-home");
        retryButton = allroundResultRoot.Q<Button>("button-retry");

        if (resultScrollView == null) { Debug.LogError("resultScrollViewが見つかりませんでした"); return; }
        if (totalScore == null) { Debug.LogError("totalScoreが見つかりませんでした"); return; }
        if (scoreRank == null) { Debug.LogError("scoreRankが見つかりませんでした"); return; }
        if (backHomeButton == null) { Debug.LogError("backHomeButtonが見つかりませんでした"); return; }
        if (retryButton == null) { Debug.LogError("retryButtonが見つかりませんでした"); return; }

        resultScrollView.Clear();   // ScrollViewの内容をクリアする
        CoreResource.ChangePanelMotion(allroundResultRoot, PanelMotionState.Hide);    // 全ラウンドの結果表示用のUIは初期状態では非表示にする

        // リプレイ
        retryButton.RegisterCallback<ClickEvent>(evt => {
            AudioManager.I.PlaySE("NormalButton");
            SceneManager.LoadScene(NameConstant.GamePlayScene);
        });

        // ホームに戻る
        backHomeButton.RegisterCallback<ClickEvent>(evt => {
            AudioManager.I.PlaySE("NormalButton");
            SceneManager.LoadScene(NameConstant.HomeScene);
        });
    }

    /// <summary>
    /// 全ラウンドの結果を表示するためのメソッド
    /// </summary>
    /// <param name="resultDataList">お題の色とPlayerが決定した色のリスト</param>
    public void ShowAllRoundResult(List<ResultData> resultDataList)
    {
        allroundResultRoot.style.display = DisplayStyle.Flex;   // 全ラウンドの結果表示用のUIを表示する
        CoreResource.ChangePanelMotion(allroundResultRoot, PanelMotionState.Show);

        // ラウンドごとの結果をScrollViewに追加する
        for (int ii = 0; ii < resultDataList.Count; ii++)
        {
            ResultData data = resultDataList[ii];
            VisualElement elem = roundResultTemplate.Instantiate();   // テンプレートからUIを生成する
            new RoundResult(elem, colorMaterials, data);    // 生成したUIにラウンドの結果を表示するための処理を行う
            resultScrollView.Add(elem);   // ScrollViewに生成したUIを追加する
        }
            
        // 総合スコアとランクの計算と表示
        int totalScoreValue = CalculateTotalScore(resultDataList);
        totalScore.text = totalScoreValue.ToString("#,###");
        // ハイスコアの更新
        CoreResource.UpdateHighScore(CoreResource.GetColorChannelData().AdjustColorGenre, totalScoreValue);   // ハイスコアの更新

        ScoreRank rank = ValueConstants.GetScoreRank(totalScoreValue);
        scoreRank.text = rank.ToString();
        scoreRank.style.unityTextOutlineColor = ValueConstants.ScoreRankColors[rank];
        scoreRankText.style.unityTextOutlineColor = ValueConstants.ScoreRankColors[rank];  

        CoreResource.ScoreHistoryDatas.Push(new ScoreHistoryData(CoreResource.GetColorChannelData(), rank, totalScoreValue, DateTime.Now));   // スコア履歴に今回のスコアを追加する
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
}