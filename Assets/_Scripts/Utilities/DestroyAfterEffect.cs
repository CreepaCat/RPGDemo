using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField] ParticleSystem ps;

    private void Awake()
    {
        if (ps == null)
        {
            ps = GetComponentInChildren<ParticleSystem>();
        }
    }

    private void Update()
    {
        if (ps.isStopped)
            Destroy(gameObject);
    }

}
