using System;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    public static HomeManager I { get; private set; }

    [SerializeField] ColorChannelDataSO colorChannelDataSO;   // 色チャンネルデータのScriptableObject
    [SerializeField] HomeUIModuleManager homeUiManager;

    [SerializeField] TopicSphereController topicSphere;   // 色のプレビューに使用する球体のRenderer
    public ColorChannelDataSO ColorChannelDataSO => colorChannelDataSO;   // 色チャンネルデータのプロパティ

    void Awake()
    {
        if(I == null) I = this;
        if(colorChannelDataSO.Datas.Count == 0) Debug.LogError("ColorChannelDataSOにデータが設定されていません。");

        CoreResource.SetColorChannelData(colorChannelDataSO.Datas[0]);   // 最初はSOの最初のデータをCoreResourceに保存する
    }

    /// <summary>
    /// 色のプレビューを更新するメソッド
    /// </summary>
    public void UpdatePreviewColor(Color color)
    {
        topicSphere.SetColor(color);     // 球体の色を更新する
        homeUiManager.ColorPreview.style.backgroundColor = color;   // UIのプレビューの色を更新する
    }

    void Start()
    {
        AudioManager.I.PlayBGM("HomeScene");
    }
}