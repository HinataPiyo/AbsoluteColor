using UnityEngine.UIElements;

public interface IPanelChangeElement
{
    VisualElement Root { get; }
    void OnPanelChange(bool isActive);
}