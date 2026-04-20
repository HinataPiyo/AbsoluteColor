using UnityEngine;
using UnityEngine.UIElements;

public class ColorPreviewPanel : MonoBehaviour, IUIModuleController, IPanelChangeElement, IBackButton
{
    public BackButton BackButton { get; private set; }
    public VisualElement Root { get; private set; }

    VisualElement adjust_RGB_Panel;    // RGB調整用のパネル

    public void Initialize(VisualElement root, Material[] Mats = null)
    {
        Root = root;

        BackButton?.Reset();        // 既存のバックボタンがあればリセットする
        adjust_RGB_Panel = root.Q<VisualElement>("rgb-adjust-color-panel");   // RGB調整用のパネルを取得する
        if(adjust_RGB_Panel == null) { Debug.LogError("rgb-adjust-color-panelのVisualElementが見つかりませんでした"); return; }
    }

    /// <summary>
    /// バックボタンのクリックイベントに処理を追加する
    /// </summary>
    public void AddRegisterBackButtonOnClick(System.Action addAction)
    {
        BackButton = new BackButton(Root, () => {
            OnBackButtonPressed();
            addAction();
        });
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

    public void OnPanelChange(bool isActive, ColorChannelData data)
    {
        OnPanelChange(isActive);    // パネルの表示/非表示を切り替える
        switch (data.AdjustColorGenre)
        {
            case AdjustColorGenre.RGB_Slider:
                adjust_RGB_Panel.style.display = isActive ? DisplayStyle.Flex : DisplayStyle.None;   // RGB調整用のパネルの表示/非表示を切り替える
                break;
            // 他の調整ジャンルが追加された場合はここにケースを追加する
        }
    }
}