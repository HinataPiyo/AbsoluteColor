using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// モード選択パネルのUIを管理するクラス
/// </summary>
public class SelectModePanel : MonoBehaviour, IUIModuleController, IPanelChangeElement, IBackButton
{
    public BackButton BackButton { get; private set; }
    public VisualElement Root { get; private set; }

    [SerializeField] VisualTreeAsset temp_SelectModePanel;   // モード選択パネルのUIテンプレート

    ScrollView modeScrollView;    // モード選択のスクロールビュー

    public void Initialize(VisualElement root, Material[] Mats = null)
    {
        Root = root;

        BackButton?.Reset();        // 既存のバックボタンがあればリセットする
        BackButton = new BackButton(root, () => OnBackButtonPressed());     // バックボタンの初期化

        modeScrollView = Root.Q<ScrollView>();   // モード選択のScrollViewを取得
        modeScrollView.Clear();    // ScrollViewの内容をクリアする

        // モード選択のUIを生成する
        foreach (ColorChannelData data in HomeManager.I.ColorChannelDataSO.Datas)
        {
            CreateModeComposition(data);   // 各モードのUIを生成する
        }
    }

    /// <summary>
    /// モード選択のUIを生成する
    /// </summary>
    void CreateModeComposition(ColorChannelData data)
    {
        VisualElement temp = temp_SelectModePanel.Instantiate();   // UIテンプレートからVisualElementを生成
        new SelectModeComposition(temp, data, () =>
        {
            // モード選択時の処理をここに記述
            SelectModeButtonClicked();
        });
        modeScrollView.Add(temp);    // ScrollViewに生成したVisualElementを追加
    }

    /// <summary>
    /// モード選択ボタンがクリックされたときの処理
    /// </summary>
    void SelectModeButtonClicked()
    {
        OnPanelChange(false);   // パネルを非表示にする
    }

    /// <summary>
    /// パネルの表示・非表示を切り替える
    /// </summary>
    /// <param name="isActive">trueなら表示、falseなら非表示</param>
    public void OnPanelChange(bool isActive)
    {
        Root.style.display = isActive ? DisplayStyle.Flex : DisplayStyle.None;
    }

    /// <summary>
    /// バックボタンが押されたときの処理
    /// </summary>
    public void OnBackButtonPressed()
    {
        OnPanelChange(false);   // パネルを非表示にする
    }

}