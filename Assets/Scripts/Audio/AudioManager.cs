using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }

    [SerializeField] AudioDataSO audioDataSO;
    [SerializeField] AudioSource seAudioSource;
    [SerializeField] AudioSource bgmAudioSource;

    void Awake()
    {
        if(I == null) I = this;
    }

    public void PlaySE(string tag)
    {
        var clip = audioDataSO.GetSEClip(tag);
        if (clip != null)
        {
            seAudioSource.PlayOneShot(clip);
        }
    }

    public void PlayBGM(string tag)
    {
        var clip = audioDataSO.GetBGMClip(tag);
        if (clip != null)
        {
            bgmAudioSource.clip = clip;
            bgmAudioSource.Play();
        }
    }

}