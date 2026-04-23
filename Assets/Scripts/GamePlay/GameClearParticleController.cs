using System.Collections.Generic;
using UnityEngine;

public class GameClearParticleController : MonoBehaviour
{
    static readonly Dictionary<ScoreRank, int> judgementToParticleAmount = new Dictionary<ScoreRank, int>()
    {
        { ScoreRank.S, 25 },
        { ScoreRank.A, 10 },
        { ScoreRank.B, 6 },
        { ScoreRank.C, 3 },
        { ScoreRank.D, 1 },
    };

    ParticleSystem[] particles;

    void Awake()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// スコアランクに応じた量のパーティクルを生成する
    /// </summary>
    public void SpawnParticles(ScoreRank rank)
    {
        gameObject.SetActive(true);
        foreach (var particle in particles)
        {
            var emission = particle.emission;
            emission.rateOverTime = judgementToParticleAmount[rank];
        }
    }
    
}