using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// モード選択パネルのUIを管理するクラス
/// </summary>
public class SelectModePanel : MonoBehaviour, IUIModuleController, IPanelChangeElement, IBackButton
{
    public BackButton BackButton { get; private set; }
    public VisualElement Root { get; private set; }

    public void Initialize(VisualElement root, Material[] Mats = null)
    {
        Root = root;
        
        BackButton?.Reset();        // 既存のバックボタンがあればリセットする
        BackButton = new BackButton(root, () => OnBackButtonPressed());     // バックボタンの初期化
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