using System;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    public static HomeManager I { get; private set; }

    [SerializeField] ColorChannelDataSO colorChannelDataSO;   // 色チャンネルデータのScriptableObject 
    public ColorChannelDataSO ColorChannelDataSO => colorChannelDataSO;    // 色チャンネルデータのプロパティ

    void Awake()
    {
        if(I == null) I = this;
        if(ColorChannelDataSO.Datas.Count == 0) Debug.LogError("ColorChannelDataSOにデータが設定されていません。");
    }

    void Start()
    {
        AudioManager.I.PlayBGM("HomeScene");
    }
}