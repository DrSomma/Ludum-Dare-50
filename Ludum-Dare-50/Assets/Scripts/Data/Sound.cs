using System;
using UnityEngine;

[Serializable]
public class Sound
{
    public string Name;

    public AudioClip AudioClip;

    [Range(min: 0f, max: 1f)]
    public float Volume = 1f;

    [Range(min: .1f, max: 3f)]
    public float Pitch = 1f;

    public bool IsLoop;
    public bool HasCooldown;
    
    [HideInInspector]
    public AudioSource AudioSource;
}