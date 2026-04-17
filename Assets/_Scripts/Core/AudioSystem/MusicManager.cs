using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.AudioSystem
{
    //[RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour
    {
        const float crossFadeTime = 1.0f;
        float fading;
        AudioSource current;
        AudioSource previous;
        readonly Queue<AudioClip> playlist = new();

        [SerializeField] List<AudioClip> initialPlaylist = null;
        [SerializeField] AudioMixerGroup musicMixerGroup = null;

        public static MusicManager Instance { get; private set; } = null;

        const string VolumePrefKey = "MusicManager.Volume";
        private float volume = 0.5f;
        public float Volume
        {
            get => volume;
            set
            {
                float newVolume = Mathf.Clamp01(value);
                if (Mathf.Approximately(volume, newVolume)) return;

                volume = newVolume;
                PlayerPrefs.SetFloat(VolumePrefKey, volume);
                PlayerPrefs.Save();
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            volume = PlayerPrefs.GetFloat(VolumePrefKey, volume);
        }

        void Start()
        {
            foreach (var clip in initialPlaylist)
            {
                AddToPlaylist(clip);
            }
        }

        public void AddToPlaylist(AudioClip clip)
        {
            Debug.Log("AddToPlaylist" + clip.name);
            playlist.Enqueue(clip);
            if (current == null && previous == null)
            {
                PlayNextTrack();
            }
        }

        public void Clear() => playlist.Clear();

        public void PlayNextTrack()
        {
            if (playlist.TryDequeue(out AudioClip nextTrack))
            {
                Play(nextTrack);
            }
        }

        public void Play(AudioClip clip)
        {
            if (current && current.clip == clip) return;

            if (previous)
            {
                Destroy(previous);
                previous = null;
            }

            previous = current;

            // 通过AddComponent来确保previous 和 current不会是同一个
            current = gameObject.AddComponent<AudioSource>();
            current.clip = clip;
            current.outputAudioMixerGroup = musicMixerGroup;
            current.loop = true; // 单曲循环
            current.volume = 0;
            current.bypassListenerEffects = true;
            current.Play();

            fading = 0.001f;
        }

        void Update()
        {
            if (!Application.isFocused) return;
            HandleCrossFade();

            if (current && !current.isPlaying && playlist.Count > 0)
            {
                PlayNextTrack();
            }
            if (current)
            {
                current.volume = Volume;
            }
        }

        void HandleCrossFade()
        {
            if (fading <= 0f) return;

            fading += Time.deltaTime;

            float fraction = Mathf.Clamp01(fading / crossFadeTime);

            // Logarithmic fade
            float logFraction = Mathf.Log10(1 + 9 * fraction) / Mathf.Log10(10);


            if (previous) previous.volume = 1.0f - logFraction;
            if (current) current.volume = logFraction;

            if (fraction >= 1)
            {
                fading = 0.0f;
                if (previous)
                {
                    Destroy(previous);
                    previous = null;
                }
            }
        }
    }
}

public static class GameObjectExtensions
{
    public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component == null)
            component = gameObject.AddComponent<T>();
        return component;
    }
}
