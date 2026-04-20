using System.Collections.Generic;
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
    List<SelectModeComposition> modeCompositions = new List<SelectModeComposition>();   // モード選択のUI構成を管理するリスト

    public void Initialize(VisualElement root, Material[] Mats = null)
    {
        Root = root;

        BackButton?.Reset();        // 既存のバックボタンがあればリセットする
        BackButton = new BackButton(root, () => OnBackButtonPressed());     // バックボタンの初期化

        modeScrollView = Root.Q<ScrollView>();   // モード選択のScrollViewを取得
        modeScrollView.Clear();    // ScrollViewの内容をクリアする


        int dataCount = 0;
        // モード選択のUIを生成する
        foreach (ColorChannelData data in HomeManager.I.ColorChannelDataSO.Datas)
        {            
            SelectModeComposition comp = CreateModeComposition(data);   // 各モードのUIを生成する
            comp.SetNowModeTagActive(dataCount == 0);   // 最初のモードのタグは表示し、他のモードのタグは非表示にする
            dataCount++;
        }
    }

    /// <summary>
    /// モード選択のUIを生成する
    /// </summary>
    SelectModeComposition CreateModeComposition(ColorChannelData data)
    {
        VisualElement temp = temp_SelectModePanel.Instantiate();   // UIテンプレートからVisualElementを生成
        SelectModeComposition composition = new SelectModeComposition(temp, data, (select) =>
        {
            // モード選択時の処理をここに記述
            SelectModeButtonClicked();
            foreach (SelectModeComposition comp in modeCompositions)
            {
                comp.SetNowModeTagActive(comp == select);   // 選択されたモードのタグを表示し、他のモードのタグは非表示にする
            }
            AudioManager.I.PlaySE("ModeChange");
        });

        modeScrollView.Add(temp);    // ScrollViewに生成したVisualElementを追加
        modeCompositions.Add(composition);   // モード選択のUI構成をリストに追加

        return composition;
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