using Core.AudioSystem;
using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] AudioClip sceneMusic;

    private void Start()
    {
        MusicManager.Instance.Clear();
        MusicManager.Instance.AddToPlaylist(sceneMusic);
        MusicManager.Instance.PlayNextTrack();
    }
}
