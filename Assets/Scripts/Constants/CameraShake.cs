using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake I { get; private set; }
    CinemachineBasicMultiChannelPerlin perlin;    // カメラシェイク用のCinemachineBasicMultiChannelPerlinコンポーネント

    void Awake()
    {
        if (I == null) I = this;
        perlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void Shake(float intensity, float duration)
    {
        StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        float elapsedTime = 0f;
        float initialIntensity = perlin.AmplitudeGain;   // シェイク開始前の強度を保存
        perlin.AmplitudeGain = intensity;    // シェイクの強度を設定
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        perlin.AmplitudeGain = initialIntensity;   // シェイク終了後に強度を元に戻す
    }

}