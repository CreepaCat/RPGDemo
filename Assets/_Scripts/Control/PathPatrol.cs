using UnityEngine;

public class PathPatrol : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    private int currentWaypoint = 0;

    private void UpdateWaypoint()
    {
        currentWaypoint++;
        currentWaypoint %= waypoints.Length;
    }

    public Transform GetCurrentWaypoint()
    {
        return waypoints[currentWaypoint];
    }

    public Transform GetNextWaypoint()
    {
        UpdateWaypoint();
        return waypoints[currentWaypoint];
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Gizmos.DrawSphere(waypoints[i].position, 0.5f);
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);

        }
        Gizmos.DrawSphere(waypoints[waypoints.Length - 1].position, 0.5f);
        Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);

    }
}
