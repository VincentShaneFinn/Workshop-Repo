using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementController : MonoBehaviour {

    public Transform Target;
    public NavMeshAgent agent;

    // Update is called once per frame
    void Update () {
        agent.SetDestination(Target.position);
    }

    public void StopMovement()
    {
        agent.speed = 0;
        agent.acceleration = 100;
    }
}
