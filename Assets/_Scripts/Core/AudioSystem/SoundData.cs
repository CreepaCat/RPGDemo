using UnityEngine;
using UnityEngine.Audio;
namespace Core.AudioSystem
{
    [System.Serializable]
    public class SoundData
    {
        public AudioClip clip;
        public AudioMixerGroup mixerGroup;
        public bool loop;
        public bool playOnAwake;
    }
}
