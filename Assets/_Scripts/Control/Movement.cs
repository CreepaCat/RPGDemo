using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Movement : MonoBehaviour
{
  private NavMeshAgent agent;

  private void Awake()
  {
    agent = GetComponent<NavMeshAgent>();
  }
}
