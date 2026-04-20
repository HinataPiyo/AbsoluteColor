using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public enum AdjustColorGenre
{
    RGB_Slider,    // RGBスライダー
}

[Serializable]
public class ColorChannelData
{
    [SerializeField] AdjustColorGenre adjustColorGenre;
    [SerializeField] Material[] materials;          // 色のマテリアルの配列
    [SerializeField, TextArea(5,10)] string description;            // モードの説明文
    public int HighScore { get; private set; }      // ハイスコアのプロパティ

    readonly Dictionary<AdjustColorGenre, string> modeName = new Dictionary<AdjustColorGenre, string>()
    {
        { AdjustColorGenre.RGB_Slider, "RGB" },
    };

    public string ModeName => modeName[adjustColorGenre];   // モードの名前のプロパティ
    public string Description => description;   // モードの説明文のプロパティ
    public AdjustColorGenre AdjustColorGenre => adjustColorGenre;   // 色のジャンルのプロパティ
    public Material[] Materials => materials;   // 色のマテリアルの配列
}

[CreateAssetMenu(fileName = "ColorChannelDataSO", menuName = "Config/ColorChannelDataSO")]
public class ColorChannelDataSO : ScriptableObject
{
    [SerializeField] List<ColorChannelData> colorChannelDatas = new List<ColorChannelData>();   // 色チャンネルデータのリスト
    public List<ColorChannelData> Datas => colorChannelDatas;   // 色チャンネルデータのリストのプロパティ
}