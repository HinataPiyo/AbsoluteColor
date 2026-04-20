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

    [SerializeField] Material[] RGB_Mats;

    protected override void Awake()
    {
        base.Awake();

        AdjustUIArea = GetComponent<AdjustUIAreaController>();
        Result = GetComponent<ResultController>();
        AllRoundTotalResult = GetComponent<AllRoundTotalResult>();
        TopicPreviewArea = GetComponent<TopicPreviewAreaController>();
    }

    void Initialize()
    {
        Initialize(AdjustUIArea, AdjustUIAreaPath, RGB_Mats);
        Initialize(Result, ResultPath, RGB_Mats);
        Initialize(AllRoundTotalResult, AllRoundTotalResultPath, RGB_Mats);
        Initialize(TopicPreviewArea, TopicPreviewAreaPath);
    }

    
    protected void OnEnable() => Initialize();
}