using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }
    public const int MaxRound = 5;
    const float initialTimeLimit = 15f;
    [SerializeField] TopicSphereController topicSphere;
    TopicPreviewAreaController topicPreviewArea;
    ResultController result;
    AdjustUIAreaController adjustUIArea;
    AllRoundTotalResult allRoundTotalResult;
    Color topicColor;
    VisualElement ignoreRoot;   // 結果表示中にプレイヤーの入力を無視するためのVisualElement
    [HideInInspector] public bool isPlayerDecided = false;
    [SerializeField] UIDocument uiDoc; 

    void Awake()
    {
        if(I == null) I = this;
        topicPreviewArea = uiDoc.GetComponent<TopicPreviewAreaController>();
        result = uiDoc.GetComponent<ResultController>();
        adjustUIArea = uiDoc.GetComponent<AdjustUIAreaController>();
        allRoundTotalResult = uiDoc.GetComponent<AllRoundTotalResult>();
    }

    void OnEnable()
    {
        ignoreRoot = uiDoc.rootVisualElement.Q<VisualElement>("result-root");
        if (ignoreRoot == null) { Debug.LogError("ignore-rootのVisualElementが見つかりませんでした"); return; }
    }

    void Start()
    {
        AudioManager.I.PlayBGM("GamePlayScene");
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        for (int ii = 1; ii <= MaxRound; ii++)
        {
            // ラウンド開始の処理
            topicPreviewArea.UpdateRoundCount(ii);      // ラウンド数の更新

            DesideTopicColor();     // お題の色を決定
            IgnorePlayerInput(false);    // ラウンド開始前はプレイヤーの入力を受け付ける

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

            if(!isPlayerDecided) { AudioManager.I.PlaySE("TimeUp"); }   // タイムアップのSEを再生する
            result.AddResultColor(topicColor, adjustUIArea.GetDecidedColor());      // 結果の色を追加する

            
            // ラウンド終了の処理
            topicPreviewArea.UpdateTimer(0f);   // タイマーを0にする
            isPlayerDecided = false;            // プレイヤーの決定フラグをリセット
            IgnorePlayerInput(true);            // ラウンド終了後はプレイヤーの入力を無視する

            // リザルトを表示してスコアを更新しPlayerが次へ行くまで待機
            yield return new WaitUntil(() => result.IsResultDisplayed == false);    // 結果が表示されている間は待機
        }

        // ゲーム終了の処理
        IgnorePlayerInput(true);    // ゲーム終了後はプレイヤーの入力を無視する
        allRoundTotalResult.ShowAllRoundResult(result.resultColors);    // 全ラウンドの結果を表示する
    }

    /// <summary>
    /// トピックの色を決定するメソッド
    /// </summary>
    void DesideTopicColor()
    {
        topicColor = new Color(GetRandomZeroToOne(), GetRandomZeroToOne(), GetRandomZeroToOne());
        topicSphere.SetColor(topicColor);
        AudioManager.I.PlaySE("SpawnTopic");
    }

    /// <summary>
    /// 0から1のランダムな値を取得するメソッド
    /// </summary>
    float GetRandomZeroToOne()
    {
        return Random.Range(0f, 1f);
    }

    /// <summary>
    /// プレイヤーの入力を無視するかどうかを切り替えるメソッド
    /// </summary>
    /// <param name="ignore">trueの場合はプレイヤーの入力を無視し、falseの場合はプレイヤーの入力を受け付ける</param>
    void IgnorePlayerInput(bool ignore)
    {
        ignoreRoot.style.display = ignore ? DisplayStyle.Flex : DisplayStyle.None;   // 無視する場合は無視用のVisualElementを表示し、受け付ける場合は非表示にする        
    }
}
