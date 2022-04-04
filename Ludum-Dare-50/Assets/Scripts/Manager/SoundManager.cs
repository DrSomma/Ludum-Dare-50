using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

namespace Manager
{
    public class SoundManager : MonoBehaviour
    {
        public enum Sounds
        {
            Theme,
            Break,
            Pickup,
            Alarm,
            AlarmTicking,
            Jump,
            Hurt
        }

        private static Dictionary<string, float> _lastTimePlayedBySoundName;

        private static List<GameObject> _currentSoundGameObjects;

        [Header("Sounds")]
        [Tooltip("Möglichkeit Sounds zu dem Spiel hinzuzufügen. Tipp: Lautet der Name 'Theme' wird dies als Theme beim Starten abgespielt.")]
        public List<Sound> SoundData;

        public static SoundManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }

            _lastTimePlayedBySoundName = new Dictionary<string, float>();
            _currentSoundGameObjects = new List<GameObject>();

            foreach (Sound sound in SoundData)
            {
                sound.AudioSource = gameObject.AddComponent<AudioSource>();
                sound.AudioSource.clip = sound.AudioClip;

                sound.AudioSource.volume = sound.Volume;
                sound.AudioSource.pitch = sound.Pitch;
                sound.AudioSource.loop = sound.IsLoop;

                if (!sound.HasCooldown)
                {
                    continue;
                }

                _lastTimePlayedBySoundName.Add(key: sound.Name, value: 0f);
            }
        }

        private void Start()
        {
            PlaySound(Sounds.Theme);
        }

        public void PlaySound(Sounds soundEnum)
        {
            Sound sound = GetSound(soundEnum);

            if (sound != null && CanPlaySound(sound))
            {
                sound.AudioSource.volume = sound.Volume;
                sound.AudioSource.pitch = sound.Pitch;
                sound.AudioSource.Play();
            }
        }

        public void PlaySound(Sounds soundEnum, bool fade)
        {
            Sound sound = GetSound(soundEnum);

            if (sound != null && CanPlaySound(sound))
            {
                if (!fade)
                {
                    PlaySound(soundEnum);
                    return;
                }

                sound.AudioSource.DOFade(endValue: sound.Volume, duration: 0.5f).From(0);
                sound.AudioSource.Play();
            }
        }

        public void PlaySound(Sounds soundEnum, Vector3 atPosition)
        {
            Sound sound = GetSound(soundEnum);

            if (sound == null || !CanPlaySound(sound))
            {
                return;
            }

            GameObject soundGameObject = new GameObject("Sound") {transform = {position = atPosition}};
            soundGameObject.transform.position = atPosition;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = sound.AudioClip;
            audioSource.volume = sound.Volume;
            audioSource.pitch = sound.Pitch;
            audioSource.Play();

            _currentSoundGameObjects.Add(soundGameObject);
            Destroy(obj: soundGameObject, t: sound.AudioClip.length);
        }

        public void StopSound(Sounds soundEnum)
        {
            GetSound(soundEnum)?.AudioSource.Stop();
        }

        public void StopSound(Sounds soundEnum, bool fade)
        {
            Sound sound = GetSound(soundEnum);
            if (sound == null)
            {
                return;
            }

            if (fade)
            {
                sound.AudioSource.DOFade(endValue: 0, duration: 0.5f).OnComplete(() => { sound.AudioSource.Stop(); });
            }
            else
            {
                StopSound(soundEnum);
            }
        }

        public Sequence StopSoundFadeOutAndPitch(Sounds soundEnum)
        {
            Sound sound = GetSound(soundEnum)!;
            Sequence sq = DOTween.Sequence();
            sq.Append(sound.AudioSource.DOFade(endValue: 0, duration: 1.5f));
            sq.Join(sound.AudioSource.DOPitch(endValue: 0.1f, duration: 1f));
            sq.OnComplete(
                () =>
                {
                    //reset sound
                    sound.AudioSource.pitch = sound.Pitch;
                    sound.AudioSource.Stop();
                });
            return sq;
        }

        public void MuteSound(Sounds soundEnum, bool newValue)
        {
            AudioSource audioSource = GetAudioSource(soundEnum);

            if (audioSource != null)
            {
                audioSource.mute = newValue;
            }
        }

        public void SetVolumeSound(Sounds soundEnum, float volume)
        {
            AudioSource audioSource = GetAudioSource(soundEnum);

            volume = Math.Max(val1: 0f, val2: Math.Min(val1: volume, val2: 1f));

            if (audioSource != null)
            {
                audioSource.volume = volume;
            }
        }

        [CanBeNull]
        private Sound GetSound(Sounds soundEnum)
        {
            string soundName = soundEnum.ToString();
            Sound sound = SoundData.FirstOrDefault(s => s.Name == soundName);

            if (sound == null)
            {
                Debug.LogError($"Sound {soundName} Not Found!");
            }

            return sound;
        }

        [CanBeNull]
        private AudioSource GetAudioSource(Sounds soundEnum)
        {
            AudioSource audioSource = GetSound(soundEnum)?.AudioSource;

            if (audioSource == null)
            {
                Debug.LogError($"AudioSource von {soundEnum} Not Found!");
            }

            return audioSource;
        }


        private static bool CanPlaySound(Sound sound)
        {
            if (!_lastTimePlayedBySoundName.TryGetValue(key: sound.Name, value: out float lastTimePlayed))
            {
                return true;
            }
            else if (lastTimePlayed + sound.AudioClip.length < Time.time)
            {
                _lastTimePlayedBySoundName[sound.Name] = Time.time;
                return true;
            }

            return false;
        }

        public void MuteAllSounds()
        {
            SoundData.ForEach(sound => sound.AudioSource.mute = true);
        }

        public void UnmuteAllSounds()
        {
            SoundData.ForEach(sound => sound.AudioSource.mute = false);
        }
    }
}