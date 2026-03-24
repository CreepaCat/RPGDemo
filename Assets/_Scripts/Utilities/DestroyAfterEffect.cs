using System;
using UnityEngine;

public class DestroyAfterEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem ps;
    public float destroyDelay = 3f;
    public bool useDelayDestroy = false;

    private void Awake()
    {
        if (ps == null)
        {
            ps = GetComponentInChildren<ParticleSystem>();
        }

        if (useDelayDestroy)
        {
            SetDestroyDelay(destroyDelay);
        }

    }

    public void SetDestroyDelay(float delay)
    {
        Destroy(gameObject, delay);
    }

    private void Update()
    {
        if (ps.isStopped)
            Destroy(gameObject);
    }

}
