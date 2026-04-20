using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class ColorPreviewPanel : MonoBehaviour, IUIModuleController, IPanelChangeElement, IBackButton
{
    public BackButton BackButton { get; private set; }
    public VisualElement Root { get; private set; }

    AdjustColorGenre curretnShowGenre;    // 現在表示している調整する色のジャンル

    AdjustRGBSlidedr adjustRGBSlidedr;    // RGB調整用のスライダー

    Dictionary<AdjustColorGenre, PanelChangeElementBase> panelChangeElements = new Dictionary<AdjustColorGenre, PanelChangeElementBase>();   // パネルの表示/非表示を切り替える要素のリスト

    public void Initialize(VisualElement root)
    {
        Root = root;
        BackButton?.Reset();        // 既存のバックボタンがあればリセットする
        adjustRGBSlidedr = new AdjustRGBSlidedr(root);   // RGB調整用のスライダーを生成する

        panelChangeElements.Add(AdjustColorGenre.RGB_Slider, adjustRGBSlidedr);   // パネルの表示/非表示を切り替える要素のリストにRGB調整用のスライダーを追加する
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
        // 現在表示している調整する色のジャンルに応じて、パネルの表示/非表示を切り替える
        panelChangeElements[curretnShowGenre].OnPanelChange(false);
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
        curretnShowGenre = data.AdjustColorGenre;   // 調整する色のジャンルを更新する
        panelChangeElements[curretnShowGenre].OnPanelChange(isActive);   // 調整する色のジャンルに応じて、パネルの表示/非表示を切り替える
    }
}