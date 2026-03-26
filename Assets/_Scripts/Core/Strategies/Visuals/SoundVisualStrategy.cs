using System.Collections.Generic;
using RPGDemo.Core.Strategies;
using UnityEngine;
[System.Serializable]
public class SoundVisualStrategy : VisualStrategy
{
    public AudioClip audioClip;
    public override void Play(Character caster, List<Character> targets, string skillId, Transform castPoint = null)
    {
        caster.GetComponent<AudioSource>().PlayOneShot(audioClip);
    }
}
