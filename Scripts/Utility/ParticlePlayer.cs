using UnityEngine;

public class ParticlePlayer : MonoBehaviour
{
    private ParticleSystem[] _allParticles;

    private void Start()
    {
        _allParticles = GetComponentsInChildren<ParticleSystem>();
    }

    public void Play()
    {
        foreach (var particle in _allParticles)
        {
            particle.Stop();
            particle.Play();
        }
    }
}
