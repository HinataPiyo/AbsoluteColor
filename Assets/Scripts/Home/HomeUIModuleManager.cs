using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(VariousButtonsController))]
[RequireComponent(typeof(SelectModePanel))]
[RequireComponent(typeof(ScoreHistoryPanel))]
[RequireComponent(typeof(ColorPreviewPanel))]
public class HomeUIModuleManager : UIModuleManagerBase
{
    const string VariousButtonsPath = "VariousButtons";
    const string SelectModePanelPath = "SelectModePanel";
    const string ScoreHistoryPanelPath = "ScoreHistoryPanel";
    const string ColorPreviewPanelPath = "ColorPreviewEditPanel";

    public VariousButtonsController VariousButtons { get; private set; }
    public SelectModePanel SelectModePanel { get; private set; }
    public ScoreHistoryPanel ScoreHistoryPanel { get; private set; }
    public ColorPreviewPanel ColorPreviewPanel { get; private set; }

    Button button_edit_rgb;
    public VisualElement ColorPreview { get; private set; }


    protected override void Awake()
    {
        base.Awake();
        VariousButtons = GetComponent<VariousButtonsController>();
        SelectModePanel = GetComponent<SelectModePanel>();
        ScoreHistoryPanel = GetComponent<ScoreHistoryPanel>();
        ColorPreviewPanel = GetComponent<ColorPreviewPanel>();

        VisualElement root = uiDoc.rootVisualElement;
        button_edit_rgb = root.Q<Button>("button-edit-rgb");
        ColorPreview = root.Q<VisualElement>("color-preview");
    }

    void Initialize()
    {
        Initialize(VariousButtons, VariousButtonsPath);
        Initialize(SelectModePanel, SelectModePanelPath);
        Initialize(ScoreHistoryPanel, ScoreHistoryPanelPath);
        Initialize(ColorPreviewPanel, ColorPreviewPanelPath);

        // モード選択ボタンのリスナーを登録する
        VariousButtons.SelectModeButton.ButtonOnClick(() => SelectModePanel.OnPanelChange(true), CoreResource.CurrentColorChannelData);
        // スコア履歴ボタンのリスナーを登録する
        VariousButtons.ScoreHistoryButtonOnClick(() => ScoreHistoryPanel.OnPanelChange(true));
        // RGB編集ボタンのリスナーを登録する
        button_edit_rgb.clicked += () => {
            // RGB編集パネルを表示する
            ColorPreviewPanel.OnPanelChange(true, HomeManager.I.ColorChannelDataSO.GetColorChannelData(AdjustColorGenre.RGB_Slider));
            VariousButtons.OnPanelChange(false);        // ボタンパネルを非表示にする
            AudioManager.I.PlaySE("NormalButton");
        };

        // RGB編集パネルのバックボタンに、ボタンパネルを表示する処理を登録する
        ColorPreviewPanel.AddRegisterBackButtonOnClick(() => VariousButtons.OnPanelChange(true));
    }

    void OnEnable() => Initialize();
}