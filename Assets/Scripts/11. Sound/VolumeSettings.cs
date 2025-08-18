using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();
    }

    public void SetMasterVolume()
    {
        float volume = volumeSlider.value;
        myMixer.SetFloat("Master", Mathf.Log10(volume)*20);
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("Music", Mathf.Log10(volume)*20);
    }
    
    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(volume)*20);
        myMixer.SetFloat("Ambience", Mathf.Log10(volume)*20);
        myMixer.SetFloat("UI", Mathf.Log10(volume)*20);
    }
}
