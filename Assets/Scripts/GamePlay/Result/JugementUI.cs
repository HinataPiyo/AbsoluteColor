using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class JugementUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI judgement_Text;
    [SerializeField] TextMeshProUGUI judgement_TextShadow;
    [SerializeField] TextMeshProUGUI parcentage_Text;
    [SerializeField] TextMeshProUGUI parcentage_TextShadow;

    [SerializeField] Volume postProcessingVolume;    // ポストプロセスボリュームの参照

    static readonly float DisplayDuration = 2f;    // ジャッジメントの表示時間
    static readonly float EffectTransitionDuration = 0.2f;

    JugementType currentJugementType;    // 現在のジャッジメントの種類を保持するプロパティ

    Bloom bloom;    // ブルームエフェクトの参照
    Vignette vignette;  // ビネットエフェクトの参照
    public class PostProcessingEffectSettings
    {
        public float bloom_intensity;
        public float vignette_intensity;
    }

    readonly Dictionary<JugementType, string> judgementTypeToText = new Dictionary<JugementType, string>()
    {
        { JugementType.Perfect, "Perfect!" },
        { JugementType.Excellent, "Excellent!" },
        { JugementType.Great, "Great!" },
        { JugementType.Good, "Good" },
        { JugementType.Bad, "Bad..." }
    };

    readonly Dictionary<JugementType, Color> judgementTypeToColor = new Dictionary<JugementType, Color>()
    {
        { JugementType.Perfect, new Color(1f, 0f, 0.9f) },
        { JugementType.Excellent, new Color(1f, 0f, 0f) },
        { JugementType.Great, new Color(1f, 6f, 0f) },
        { JugementType.Good, new Color(0f, 1f, 0f) },
        { JugementType.Bad, new Color(0.1f, 0.1f, 0.2f) }
    };

    readonly Dictionary<JugementType, PostProcessingEffectSettings> judgementTypeToPostProcessingEffect = new Dictionary<JugementType, PostProcessingEffectSettings>()
    {
        { JugementType.Perfect, new PostProcessingEffectSettings { bloom_intensity = 20, vignette_intensity = 0.2f } },
        { JugementType.Excellent, new PostProcessingEffectSettings { bloom_intensity = 15, vignette_intensity = 0.2f } },
        { JugementType.Great, new PostProcessingEffectSettings { bloom_intensity = 10, vignette_intensity = 0.2f } },
        { JugementType.Good, new PostProcessingEffectSettings { bloom_intensity = 1.5f, vignette_intensity = 0.25f } },
        { JugementType.Bad, new PostProcessingEffectSettings { bloom_intensity = -1, vignette_intensity = 0.3f } }
    };

    void Awake()
    {
        SetActive(false);
        if (postProcessingVolume != null)
        {
            postProcessingVolume.profile.TryGet(out bloom);
            postProcessingVolume.profile.TryGet(out vignette);
        }
        else
        {
            Debug.LogError("Post-processing volume is not assigned in the inspector.");
        }
    }

    /// <summary>
    /// UIの表示・非表示を切り替えるメソッド
    /// </summary>
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    /// <summary>
    /// ジャッジメントのテキストとパーセンテージを設定するメソッド
    /// </summary>
    public void SetJugementText(JugementType type, float parcentage)
    {
        currentJugementType = type;    // 現在のジャッジメントの種類を更新する

        judgement_Text.text = judgementTypeToText[type];
        judgement_TextShadow.text = judgementTypeToText[type];
        parcentage_Text.text = parcentage.ToString("F1") + "%";
        parcentage_TextShadow.text = parcentage.ToString("F1") + "%";

        Color color = judgementTypeToColor[type];
        judgement_TextShadow.color = color;
        parcentage_TextShadow.color = color;

        CameraShake.I.Shake(2f, 0.1f);   // ジャッジメントの種類に応じてカメラを揺らす
        JugementTypeToSE(type);  // ジャッジメントの種類に応じたSEを再生する
    }

    public IEnumerator DisplayAndHide()
    {
        SetActive(true);
        StartCoroutine(AssignPostProcessingEffect(currentJugementType));   // ジャッジメントの種類に応じたポストプロセスエフェクトを適用するコルーチンを開始する
        yield return new WaitForSeconds(DisplayDuration);
        SetActive(false);
    }

    public void ResetPostProcessingEffect()
    {
        bloom.intensity.value = 0f;
        vignette.intensity.value = 0f;
    }

    IEnumerator AssignPostProcessingEffect(JugementType type)
    {
        // 値の変更を滑らかにするためのコルーチン
        PostProcessingEffectSettings settings = judgementTypeToPostProcessingEffect[type];
        float elapsedTime = 0f;
        float initialBloomIntensity = bloom.intensity.value;
        float initialVignetteIntensity = vignette.intensity.value;

        // ジャッジメントの種類に応じてブルームとビネットの強度を線形補間で変更する
        while (elapsedTime < EffectTransitionDuration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / EffectTransitionDuration);
            bloom.intensity.value = Mathf.Lerp(initialBloomIntensity, settings.bloom_intensity, t);
            vignette.intensity.value = Mathf.Lerp(initialVignetteIntensity, settings.vignette_intensity, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bloom.intensity.value = settings.bloom_intensity;
        vignette.intensity.value = settings.vignette_intensity;
    }

    void JugementTypeToSE(JugementType type)
    {
        switch (type)
        {
            case JugementType.Perfect:
                AudioManager.I.PlaySE("Jugement_Perfect");
                break;
            case JugementType.Excellent:
                AudioManager.I.PlaySE("Jugement_Excellent");
                break;
            case JugementType.Great:
                AudioManager.I.PlaySE("Jugement_Great");
                break;
            case JugementType.Good:
                AudioManager.I.PlaySE("Jugement_Good");
                break;
            case JugementType.Bad:
                AudioManager.I.PlaySE("Jugement_Bad");
                break;
        }
    }
}