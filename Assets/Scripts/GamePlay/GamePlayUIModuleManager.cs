using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
[RequireComponent(typeof(AdjustUIAreaController))]
[RequireComponent(typeof(ResultController))]
[RequireComponent(typeof(AllRoundTotalResult))]
[RequireComponent(typeof(TopicPreviewAreaController))]
public class GamePlayUIModuleManager : MonoBehaviour
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

    UIDocument uiDoc;

    void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        AdjustUIArea = GetComponent<AdjustUIAreaController>();
        Result = GetComponent<ResultController>();
        AllRoundTotalResult = GetComponent<AllRoundTotalResult>();
        TopicPreviewArea = GetComponent<TopicPreviewAreaController>();
    }

    void Initialize()
    {
        Initialize(AdjustUIArea, AdjustUIAreaPath);
        Initialize(Result, ResultPath);
        Initialize(AllRoundTotalResult, AllRoundTotalResultPath);
        Initialize(TopicPreviewArea, TopicPreviewAreaPath);
    }

    void Initialize(IUIModuleController module, string path)
    {
        var root = uiDoc.rootVisualElement.Q<VisualElement>(path);
        if (root == null) { Debug.LogError($"{path}のVisualElementが見つかりませんでした"); return; }
        module.Initialize(root, RGB_Mats);
    }

    void OnEnable() => Initialize();
}