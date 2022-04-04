using System;
using GD.MinMaxSlider;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Sound
{
    public string Name;

    public AudioClip AudioClip;

    [Range(min: 0f, max: 1f)]
    public float Volume = 1f;

    [Range(min: .1f, max: 3f)]
    [SerializeField]
    private float pitch = 1f;
    
    public float Pitch => HasPitchVariante ? pitch + Random.Range(PitchVariante.x,PitchVariante.y) : pitch;
    public bool HasPitchVariante;
    
    [MinMaxSlider(-3,3)] 
    public Vector2 PitchVariante;

    public bool IsLoop;
    public bool HasCooldown;
    
    [HideInInspector]
    public AudioSource AudioSource;
}