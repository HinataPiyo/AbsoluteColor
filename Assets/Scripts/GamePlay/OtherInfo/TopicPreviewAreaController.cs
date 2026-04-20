using UnityEngine;
using UnityEngine.UIElements;

public class TopicPreviewAreaController : MonoBehaviour, IUIModuleController
{
    Label maxScore;
    Label roundCount;
    Label timer;

    public void Initialize(VisualElement root)
    {
        maxScore = root.Q("TopicPreviewArea").Q<Label>("max-score");
        roundCount = root.Q("TopicPreviewArea").Q<Label>("round-count");
        timer = root.Q("TopicPreviewArea").Q<Label>("timer");

        if (maxScore == null) { Debug.LogError("max-scoreのLabelが見つかりませんでした"); return; }
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

    public void UpdateMaxScore(int score)
    {
        // 最高スコアの更新処理
        maxScore.text = "HightScore: " + score.ToString();
    }
}