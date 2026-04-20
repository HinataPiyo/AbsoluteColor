using UnityEngine;

[RequireComponent(typeof(VariousButtonsController))]
[RequireComponent(typeof(SelectModePanel))]
[RequireComponent(typeof(ScoreHistoryPanel))]
public class HomeUIModuleManager : UIModuleManagerBase
{
    const string VariousButtonsPath = "VariousButtons";
    const string SelectModePanelPath = "SelectModePanel";
    const string ScoreHistoryPanelPath = "ScoreHistoryPanel";

    public VariousButtonsController VariousButtons { get; private set; }
    public SelectModePanel SelectModePanel { get; private set; }
    public ScoreHistoryPanel ScoreHistoryPanel { get; private set; }


    protected override void Awake()
    {
        base.Awake();
        VariousButtons = GetComponent<VariousButtonsController>();
        SelectModePanel = GetComponent<SelectModePanel>();
        ScoreHistoryPanel = GetComponent<ScoreHistoryPanel>();
    }

    void Initialize()
    {
        Initialize(VariousButtons, VariousButtonsPath);
        Initialize(SelectModePanel, SelectModePanelPath);
        Initialize(ScoreHistoryPanel, ScoreHistoryPanelPath);

        // モード選択ボタンのリスナーを登録する
        VariousButtons.SelectModeButton.ButtonOnClick(() => SelectModePanel.OnPanelChange(true), CoreResource.CurrentColorChannelData);
        // スコア履歴ボタンのリスナーを登録する
        VariousButtons.ScoreHistoryButtonOnClick(() => ScoreHistoryPanel.OnPanelChange(true));
    }

    void OnEnable() => Initialize();
}