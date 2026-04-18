using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }
    const int MaxRound = 5;
    const float initialTimeLimit = 15f;
    [SerializeField] TopicSphereController topicSphere;
    [SerializeField] TopicPreviewAreaController topicPreviewArea;
    [SerializeField] ResultController result;
    Color topicColor;
    bool isPlayerDecided = false;

    void Awake()
    {
        if(I == null) I = this;
    }

    void Start()
    {
        StartCoroutine(GameLoop());
    }

    void Update()
    {
        
    }

    IEnumerator GameLoop()
    {
        for (int ii = 1; ii <= MaxRound; ii++)
        {
            // ラウンド開始の処理
            topicPreviewArea.UpdateRoundCount(ii);      // ラウンド数の更新

            DesideTopicColor();     // お題の色を決定

            // Mathf.Powとは、第一引数を第二引数乗した値を返す関数です。
            // ここでは、ラウンドが進むごとに時間制限を短くするために使用しています。
            float timeLimit = initialTimeLimit * Mathf.Pow(0.9f, ii - 1);   // ラウンドごとに5%ずつ短くする
            float elapsedTime = 0f;

            while (elapsedTime < timeLimit)
            {
                // タイマーの更新
                topicPreviewArea.UpdateTimer(timeLimit - elapsedTime);
                elapsedTime += Time.deltaTime;
                if(isPlayerDecided) break;   // プレイヤーが色を決定したらループを抜ける
                yield return null;
            }

            // ラウンド終了の処理
            topicPreviewArea.UpdateTimer(0f);   // タイマーを0にする
            isPlayerDecided = false;    // プレイヤーの決定フラグをリセット

            // リザルトを表示してスコアを更新しPlayerが次へ行くまで待機
        }

        // ゲーム終了の処理
        Debug.Log("Game Over");
    }

    /// <summary>
    /// トピックの色を決定するメソッド
    /// </summary>
    void DesideTopicColor()
    {
        topicColor = new Color(GetRandomZeroToOne(), GetRandomZeroToOne(), GetRandomZeroToOne());
        topicSphere.SetColor(topicColor);
    }

    /// <summary>
    /// 0から1のランダムな値を取得するメソッド
    /// </summary>
    float GetRandomZeroToOne()
    {
        return Random.Range(0f, 1f);
    }

    /// <summary>
    /// Playerが色を決定したときに呼び出されるメソッド
    /// </summary>
    public void OnDecideColor(float r, float g, float b)
    {
        result.AddResultColor(topicColor, new Color(r, g, b));
        isPlayerDecided = true;
    }
}
