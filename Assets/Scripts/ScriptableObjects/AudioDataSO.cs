using UnityEngine;

[CreateAssetMenu(fileName = "AudioDataSO", menuName = "Config/AudioDataSO")]
public class AudioDataSO : ScriptableObject
{
    [SerializeField] ClipData[] seClips;
    [SerializeField] ClipData[] bgmClips;

    public AudioClip GetSEClip(string tag)
    {
        foreach (var clipData in seClips)
        {
            if (clipData.tag == tag)
            {
                return clipData.clip;
            }
        }
        Debug.LogWarning($"SE with tag {tag} not found!");
        return null;
    }

    public AudioClip GetBGMClip(string tag)
    {
        foreach (var clipData in bgmClips)
        {
            if (clipData.tag == tag)
            {
                return clipData.clip;
            }
        }
        Debug.LogWarning($"BGM with tag {tag} not found!");
        return null;
    }


    [System.Serializable]
    public class ClipData
    {
        public string tag;
        public AudioClip clip;
    }
}