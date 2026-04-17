using RPGDemo.Attributes;
using UnityEngine;

public class DeathCollider : MonoBehaviour
{
    public bool onlyAffectPlayer = true;
    private void OnCollisionEnter(Collision other)
    {
        if (onlyAffectPlayer)
        {
            if (!other.transform.CompareTag("Player"))
            {
                return;
            }
        }
        if (other.transform.TryGetComponent(out Health health))
        {
            health.TakeDamage(health.GetMaxHealth());
        }
    }
}
