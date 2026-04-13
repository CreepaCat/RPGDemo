using UnityEngine;
namespace Core.AudioSystem
{
    public class SoundBuilder
    {
        readonly SoundManager soundManager;
        SoundData soundData;
        Vector3 playPosition = Vector3.zero;
        bool randomPitch;

        public SoundBuilder(SoundManager soundManager)
        {
            this.soundManager = soundManager;
        }

        public SoundBuilder WithSound(SoundData data)
        {
            this.soundData = data;
            return this;
        }

        public SoundBuilder WithPlayPosition(Vector3 position)
        {
            playPosition = position;
            return this;
        }

        public SoundBuilder WithRandomPitch()
        {
            randomPitch = true;
            return this;
        }

        public void Play()
        {
            if (!soundManager.CanPlaySound(soundData)) return;

            SoundEmitter soundEmitter = soundManager.Get();
            soundEmitter.Initialize(soundData);
            soundEmitter.SetVolume(soundManager.Volume);
            soundEmitter.transform.position = playPosition;
            soundEmitter.transform.SetParent(soundManager.transform);
            if (randomPitch)
            {
                soundEmitter.WithRandomPitch();
            }

            //记录同种声音个数
            if (soundManager.Counts.TryGetValue(soundData, out int count))
            {
                soundManager.Counts[soundData] = count + 1;
            }
            else
            {
                soundManager.Counts[soundData] = 1;
            }

            soundEmitter.Play();

        }



    }
}
