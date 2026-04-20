using UnityEngine.UIElements;
using UnityEngine;
using System;

/// <summary>
/// BackButtonを実装する際に必要なプロパティとメソッドを定義するインターフェース
/// </summary>
public interface IBackButton {
    BackButton BackButton { get; }
    void OnBackButtonPressed();
}

/// <summary>
/// バックボタンの機能を提供するクラス
/// </summary>
public class BackButton
{
    Button backButton;
    Action action;

    /// <summary>
    /// バックボタンを初期化するコンストラクタ
    /// </summary>
    public BackButton(VisualElement root, Action onBack)
    {
        action = onBack;
        backButton = root.Q<VisualElement>("BackButton").Q<Button>();
        backButton.clicked += () => action();
    }

    /// <summary>
    /// ボタンのクラスがインスタンス化されていれば一度破棄する
    /// これにより、同じUIモジュールが再度表示されたときにバックボタンの機能が重複してしまうのを防ぐ
    /// </summary>
    public void Reset()
    {
        if(action == null) Debug.LogError("BackButtonのActionが設定されていません。");

        if(backButton != null)
        {
            backButton.clicked -= () => action();
            backButton = null;
        }
    }
}