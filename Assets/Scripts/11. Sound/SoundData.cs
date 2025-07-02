using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio/SoundData")]
public class SoundData : ScriptableObject
{
    public string soundID;
    public AudioClip[] clips;
    public AudioMixerGroup outputGroup;
    public float volume = 1f;
    public bool loop = false;
}