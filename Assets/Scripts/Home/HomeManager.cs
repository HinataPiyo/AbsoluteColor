using System;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    public static HomeManager I { get; private set; }

    [SerializeField] ColorChannelDataSO colorChannelDataSO;   // 色チャンネルデータのScriptableObject
    public ColorChannelDataSO ColorChannelDataSO => colorChannelDataSO;   // 色チャンネルデータのプロパティ

    void Awake()
    {
        if(I == null) I = this;
        if(colorChannelDataSO.Datas.Count == 0) Debug.LogError("ColorChannelDataSOにデータが設定されていません。");

        CoreResource.CurrentColorChannelData = colorChannelDataSO.Datas[0];   // 最初の色チャンネルデータを現在のデータとして設定
    }

    void Start()
    {
        AudioManager.I.PlayBGM("HomeScene");
    }
}