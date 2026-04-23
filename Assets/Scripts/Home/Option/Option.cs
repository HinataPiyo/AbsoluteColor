using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class Option : MonoBehaviour, IUIModuleController, IPanelChangeElement, IBackButton
{
    public BackButton BackButton { get; private set; }
    public VisualElement Root { get; private set; }

    Slider bgmSlider;
    Slider seSlider;

    [SerializeField] AudioMixer audioMixer;    // オーディオミキサーの参照

    public void Initialize(VisualElement root)
    {
        Root = root;

        BackButton?.Reset();        // 既存のバックボタンがあればリセットする

        bgmSlider = Root.Q<VisualElement>("adjust-bgm").Q<Slider>();   // BGM調整のスライダーを取得
        seSlider = Root.Q<VisualElement>("adjust-se").Q<Slider>();     // SE調整のスライダーを取得

        // スライダーの値をオーディオミキサーの現在の設定に合わせる
        audioMixer.GetFloat("BGMVolume", out float bgmVolume);
        audioMixer.GetFloat("SEVolume", out float seVolume);
        bgmSlider.highValue = 20f;
        seSlider.highValue = 20f;
        bgmSlider.lowValue = -80f;
        seSlider.lowValue = -80f;
        bgmSlider.value = bgmVolume;
        seSlider.value = seVolume;

        // スライダーの値が変更されたときのイベントリスナーを追加
        bgmSlider.RegisterValueChangedCallback(evt =>
        {
            audioMixer.SetFloat("BGMVolume", evt.newValue);   // BGMの音量を変更
            AudioManager.I.PlaySE("NormalButton");    // 音量変更のSEを再生
        });

        seSlider.RegisterValueChangedCallback(evt =>
        {
            audioMixer.SetFloat("SEVolume", evt.newValue);   // SEの音量を変更
            AudioManager.I.PlaySE("NormalButton");    // 音量変更のSEを再生
        });
    }

    /// <summary>
    /// バックボタンが押されたときの処理
    /// </summary>
    public void OnBackButtonPressed()
    {
        OnPanelChange(false);    // パネルを非表示にする
        AudioManager.I.PlaySE("BackButton");
    }
    public void AddRegisterBackButtonOnClick(System.Action addAction)
    {
        BackButton = new BackButton(Root, () => {
            OnBackButtonPressed();
            addAction();
        });
    }

    /// <summary>
    /// パネルの表示/非表示を切り替える処理
    /// </summary>
    public void OnPanelChange(bool isActive)
    {
        Root.style.display = isActive ? DisplayStyle.Flex : DisplayStyle.None;   // パネルの表示/非表示を切り替える
    }
}