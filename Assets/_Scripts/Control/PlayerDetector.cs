using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    Transform player;


    public Vector3 GetPlayerPosition()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        return player.position;
    }
    public float DistanceToPlayer()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        return Vector3.Distance(player.position, transform.position);
    }
}
