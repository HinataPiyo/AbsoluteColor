using UnityEngine;

[RequireComponent(typeof(VariousButtonsController))]
[RequireComponent(typeof(SelectModePanel))]
public class HomeUIModuleManager : UIModuleManagerBase
{
    const string VariousButtonsPath = "VariousButtons";
    const string SelectModePanelPath = "SelectModePanel";
    public VariousButtonsController VariousButtons { get; private set; }
    public SelectModePanel SelectModePanel { get; private set; }


    protected override void Awake()
    {
        base.Awake();
        VariousButtons = GetComponent<VariousButtonsController>();
        SelectModePanel = GetComponent<SelectModePanel>();
    }

    void Initialize()
    {
        Initialize(VariousButtons, VariousButtonsPath);
        Initialize(SelectModePanel, SelectModePanelPath);

        // モード選択パネルを表示
        VariousButtons.SelectModeButtonOnClick(() => SelectModePanel.OnPanelChange(true));
    }

    void OnEnable() => Initialize();
}