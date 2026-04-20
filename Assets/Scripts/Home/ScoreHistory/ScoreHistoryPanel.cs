using UnityEngine;
using UnityEngine.UIElements;

public class ScoreHistoryPanel : MonoBehaviour, IUIModuleController, IPanelChangeElement, IBackButton
{
    const int MaxScoreHistoryCount = 100;    // 表示するスコア履歴の最大数
    public BackButton BackButton { get; private set; }
    public VisualElement Root { get; private set; }
    [SerializeField] VisualTreeAsset temp_ScoreHistoryComposition;

    ScrollView scoreHistoryScrollView;    // モード選択のスクロールビュー
    

    public void Initialize(VisualElement root)
    {
        Root = root;

        BackButton?.Reset();        // 既存のバックボタンがあればリセットする
        BackButton = new BackButton(root, () => OnBackButtonPressed());     // バックボタンの初期化

        scoreHistoryScrollView = Root.Q<ScrollView>();   // スコア履歴のScrollViewを取得
        scoreHistoryScrollView.Clear();    // ScrollViewの内容をクリアする

        int historyCount = 0;
        // スコア履歴のUIを生成する
        foreach (ScoreHistoryData data in CoreResource.ScoreHistoryDatas)
        {
            if(historyCount >= MaxScoreHistoryCount) break;  // 最大数に達したらループを抜ける

            VisualElement temp = temp_ScoreHistoryComposition.Instantiate();   // UIテンプレートからVisualElementを生成
            ScoreHistoryComposition composition = new ScoreHistoryComposition(temp, data, () =>
            {
                // スコア履歴がクリックされたときの処理をここに記述
                AudioManager.I.PlaySE("NormalButton");
            });

            scoreHistoryScrollView.Add(temp);    // ScrollViewに生成したVisualElementを追加

            historyCount++;
        }
    }

    /// <summary>
    /// バックボタンが押されたときの処理
    /// </summary>
    public void OnBackButtonPressed()
    {
        OnPanelChange(false);    // パネルを非表示にする
        AudioManager.I.PlaySE("BackButton");
    }

    /// <summary>
    /// パネルの表示/非表示を切り替える処理
    /// </summary>
    public void OnPanelChange(bool isActive)
    {
        Root.style.display = isActive ? DisplayStyle.Flex : DisplayStyle.None;   // パネルの表示/非表示を切り替える
    }
}