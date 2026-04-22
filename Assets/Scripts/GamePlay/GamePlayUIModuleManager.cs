using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
[RequireComponent(typeof(AdjustUIAreaController))]
[RequireComponent(typeof(ResultController))]
[RequireComponent(typeof(AllRoundTotalResult))]
[RequireComponent(typeof(TopicPreviewAreaController))]
public class GamePlayUIModuleManager : UIModuleManagerBase
{
    public AdjustUIAreaController AdjustUIArea { get; private set; } 
    public ResultController Result { get; private set; }
    public AllRoundTotalResult AllRoundTotalResult { get; private set; }
    public TopicPreviewAreaController TopicPreviewArea { get; private set; }


    const string AdjustUIAreaPath = "AdjustColorArea";
    const string TopicPreviewAreaPath = "TopicPreviewArea";
    const string ResultPath = "RoundResultScore";
    const string AllRoundTotalResultPath = "AllRoundResultScore";

    Material[] ColorMaterials;

    protected override void Awake()
    {
        base.Awake();

        AdjustUIArea = GetComponent<AdjustUIAreaController>();
        Result = GetComponent<ResultController>();
        AllRoundTotalResult = GetComponent<AllRoundTotalResult>();
        TopicPreviewArea = GetComponent<TopicPreviewAreaController>();

        ColorMaterials = CoreResource.GetColorChannelData().Materials;   // 現在の色チャンネルデータからマテリアルを取得
    }

    void Initialize()
    {
        Initialize(AdjustUIArea, AdjustUIAreaPath, ColorMaterials);
        Initialize(Result, ResultPath, ColorMaterials);
        Initialize(AllRoundTotalResult, AllRoundTotalResultPath, ColorMaterials);
        Initialize(TopicPreviewArea, TopicPreviewAreaPath);
    }

    
    protected void OnEnable() => Initialize();
}