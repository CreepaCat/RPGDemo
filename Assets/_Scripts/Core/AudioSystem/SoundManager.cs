using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
namespace Core.AudioSystem
{
    public class SoundManager : MonoBehaviour
    {
        // private static SoundManager _instance = null;
        public static SoundManager Instance { get; private set; }

        const string VolumePrefKey = "SoundManager.Volume";
        float volume = 0.5f;
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

        IObjectPool<SoundEmitter> soundEmitterPool;
        readonly List<SoundEmitter> activedSoundEmitters = new();
        public Dictionary<SoundData, int> Counts = new();

        [SerializeField] SoundEmitter soundEmitterPrefab;
        [SerializeField] bool collectionCheck = true;
        [SerializeField] int defaultCapacity = 10;
        [SerializeField] int maxPoolSize = 100;

        //同种声音允许存在的最大数量
        [SerializeField] int maxSoundInstances = 30;

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

        private void Start()
        {
            InitializePool();
        }

        public SoundBuilder CreateSound() => new SoundBuilder(this);

        public bool CanPlaySound(SoundData data)
        {
            if (Counts.TryGetValue(data, out var count))
            {
                if (count >= maxSoundInstances)
                    return false;
            }
            return true;
        }

        public SoundEmitter Get()
        {
            return soundEmitterPool.Get();
        }

        public void ReturnToPool(SoundEmitter soundEmitter)
        {
            //更新记录的声音个数
            if (Counts.TryGetValue(soundEmitter.Data, out int count))
            {
                Counts[soundEmitter.Data] -= count > 0 ? 1 : 0;
            }
            soundEmitterPool.Release(soundEmitter);
        }


        #region 对象池相关

        private void InitializePool()
        {
            soundEmitterPool = new ObjectPool<SoundEmitter>(
            CreateSoundEmitter,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            collectionCheck,
            defaultCapacity,
            maxPoolSize
            );
        }

        private void OnDestroyPoolObject(SoundEmitter emitter)
        {
            Destroy(emitter.gameObject);
        }

        private void OnReturnedToPool(SoundEmitter emitter)
        {
            emitter.gameObject.SetActive(false);
            activedSoundEmitters.Remove(emitter);
        }

        private void OnTakeFromPool(SoundEmitter emitter)
        {
            emitter.gameObject.SetActive(true);
            activedSoundEmitters.Add(emitter);
        }

        private SoundEmitter CreateSoundEmitter()
        {
            SoundEmitter soundEmitter = Instantiate(soundEmitterPrefab);
            soundEmitter.gameObject.SetActive(true);
            return soundEmitter;
        }
        #endregion
    }
}
