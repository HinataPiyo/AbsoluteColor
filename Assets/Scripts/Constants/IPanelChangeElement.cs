using UnityEngine.UIElements;

/// <summary>
/// パネルの表示/非表示を切り替えるためのインターフェース
/// </summary>
public interface IPanelChangeElement
{
    VisualElement Root { get; }
    void OnPanelChange(bool isActive);
}

/// <summary>
/// IPanelChangeElementの基本実装を提供するクラス
/// </summary>
public class PanelChangeElementBase
{
    public VisualElement Root { get; protected set; }

    public virtual void OnPanelChange(bool isActive)
    {
        Root.style.display = isActive ? DisplayStyle.Flex : DisplayStyle.None;   // パネルの表示/非表示を切り替える
    }
}