using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource sfxSourcePrefab;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private List<SoundData> sounds;

    private Dictionary<string, SoundData> soundLookup;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        soundLookup = new Dictionary<string, SoundData>();
        foreach (var s in sounds)
        {
            soundLookup[s.soundID] = s;
        }
    }

    public void PlaySFX(string soundID, Vector3? position = null)
    {
        if (!soundLookup.TryGetValue(soundID, out SoundData data))
        {
            Debug.LogWarning("Sound not found: " + soundID);
            return;
        }

        if (data.clips == null || data.clips.Length == 0)
        {
            Debug.LogWarning("No clips assigned for soundID: " + soundID);
            return;
        }

        var source = Instantiate(sfxSourcePrefab, transform);
        source.outputAudioMixerGroup = data.outputGroup;

        AudioClip chosenClip = data.clips[Random.Range(0, data.clips.Length)];
        source.clip = chosenClip;
        source.volume = data.volume;
        source.loop = data.loop;
        
        if (position.HasValue)
        {
            source.transform.position = position.Value;
            source.spatialBlend = 0.5f; 
        }
        else
        {
            source.transform.position = Vector3.zero;
            source.spatialBlend = 0f; 
        }
        
        source.Play();

        if (!data.loop)
            Destroy(source.gameObject, chosenClip.length);
    }

    public void PlayMusic(string soundID)
    {
        if (!soundLookup.TryGetValue(soundID, out SoundData data))
        {
            Debug.LogWarning("Music not found: " + soundID);
            return;
        }

        if (data.clips == null || data.clips.Length == 0)
        {
            Debug.LogWarning("No music clips assigned for soundID: " + soundID);
            return;
        }

        AudioClip chosenClip = data.clips[0]; 
        musicSource.clip = chosenClip;
        musicSource.outputAudioMixerGroup = data.outputGroup;
        musicSource.volume = data.volume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
    
    public void FadeMusicVolume(float targetVolume, float duration)
    {
        targetVolume = Mathf.Clamp01(targetVolume);
        StartCoroutine(FadeVolumeCoroutine(targetVolume, duration));
    }

    private IEnumerator FadeVolumeCoroutine(float targetVolume, float duration)
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }
        musicSource.volume = targetVolume;
    }
}
