using UnityEngine;
using UnityEngine.UIElements;

public class TopicPreviewAreaController : MonoBehaviour
{
    UIDocument uiDoc;

    Label maxScore, nowScore;
    Label roundCount;
    Label timer;

    void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        maxScore = uiDoc.rootVisualElement.Q("TopicPreviewArea").Q<Label>("max-score");
        nowScore = uiDoc.rootVisualElement.Q("TopicPreviewArea").Q<Label>("now-score");
        roundCount = uiDoc.rootVisualElement.Q("TopicPreviewArea").Q<Label>("round-count");
        timer = uiDoc.rootVisualElement.Q("TopicPreviewArea").Q<Label>("timer");

        if (maxScore == null) { Debug.LogError("max-scoreのLabelが見つかりませんでした"); return; }
        if (nowScore == null) { Debug.LogError("now-scoreのLabelが見つかりませんでした"); return; }
        if (roundCount == null) { Debug.LogError("round-countのLabelが見つかりませんでした"); return; }
        if (timer == null) { Debug.LogError("timerのLabelが見つかりませんでした"); return; }
    }

    public void UpdateTimer(float time)
    {
        // タイマーの更新処理
        timer.text = time.ToString("F2") + "s";
    }

    public void UpdateRoundCount(int count)
    {
        // ラウンド数の更新処理
        roundCount.text = "Round " + count.ToString();
    }

    public void UpdateNowScore(int score)
    {
        // 現在のスコアの更新処理
        nowScore.text = "NowScore: " + score.ToString();
    }

    public void UpdateMaxScore(int score)
    {
        // 最高スコアの更新処理
        maxScore.text = "MaxScore: " + score.ToString();
    }
}