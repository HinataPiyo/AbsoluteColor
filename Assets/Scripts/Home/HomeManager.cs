using UnityEngine;

public class HomeManager : MonoBehaviour
{
    public static HomeManager I { get; private set; }

    void Awake()
    {
        if(I == null) I = this;
    }

    void Start()
    {
        AudioManager.I.PlayBGM("HomeScene");
    }
}