using UnityEngine;
using UnityEngine.UIElements;

public class UIModuleManagerBase : MonoBehaviour
{
    protected UIDocument uiDoc;

    protected virtual void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
    }

    protected void Initialize(IUIModuleController module, string path, Material[] RGB_Mats = null)
    {
        var root = uiDoc.rootVisualElement.Q<VisualElement>(path);
        if (root == null) { Debug.LogError($"{path}のVisualElementが見つかりませんでした"); return; }
        module.Initialize(root, RGB_Mats);
    }
}