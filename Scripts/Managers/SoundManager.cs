using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private bool musicEnabled = true;
    [SerializeField] private bool fxEnabled = true;
    [Range(0, 1)] [SerializeField] private float musicVolume = 1f;
    [Range(0, 1)] [SerializeField] private float fxVolume = 1f;

    [SerializeField] private List<Sound> fxSounds;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private AudioClip[] vocalClips;
    
    [SerializeField] private IconToggle fxIconToggle;
    [SerializeField] private IconToggle musicIconToggle;

    private Camera _mainCamera;
    private Dictionary<SoundName, AudioClip> _audioClipsDictionary;

    private void Start()
    {
        PlayBackgroundMusic(GetRandomClip(musicClips));
        _mainCamera = Camera.main;
        _audioClipsDictionary = fxSounds.ToDictionary(k => k.SoundName, v => v.AudioClip);
        ToggleIcon(fxIconToggle, fxEnabled);
        ToggleIcon(musicIconToggle, musicEnabled);
    }

    private static AudioClip GetRandomClip(AudioClip[] audioClips)
    {
        return audioClips[Random.Range(0, audioClips.Length)];
    }

    private void PlayBackgroundMusic(AudioClip musicClip)
    {
        if (!musicEnabled || !musicClip || !musicSource) return;
        
        musicSource.Stop();
        musicSource.clip = musicClip;
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayFx(SoundName soundName, float volumeMultiplier = 1f)
    {
        var fxClip = _audioClipsDictionary[soundName];
        if (!fxEnabled || !fxClip || !_mainCamera) return;

        var volume = Mathf.Clamp(fxVolume * volumeMultiplier, 0.05f, 1f);
        AudioSource.PlayClipAtPoint(fxClip, _mainCamera.transform.position, volume);
    }

    public void PlayRandomVocal()
    {
        if (!fxEnabled) return;
        AudioSource.PlayClipAtPoint(GetRandomClip(vocalClips), _mainCamera.transform.position);
    }

    private void UpdateMusic()
    {
        if (musicSource.isPlaying == musicEnabled) return;
        if (musicEnabled)
        {
            PlayBackgroundMusic(GetRandomClip(musicClips));
        }
        else
        {
            musicSource.Stop();
        }
    }

    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;
        UpdateMusic();
        ToggleIcon(musicIconToggle, musicEnabled);
    }

    public void ToggleFX()
    {
        fxEnabled = !fxEnabled;
        ToggleIcon(fxIconToggle, fxEnabled);
    }

    private static void ToggleIcon(IconToggle iconToggle, bool state)
    {
        if (iconToggle)
        {
            iconToggle.ToggleIcon(state);
        }
    }
}
