using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementController : MonoBehaviour {

    public Transform Target;
    public NavMeshAgent agent;
    private float savedSpeed;
    private float savedAcc;
    private float pauseTime = 1;
    private float pauseCount;

    void Start()
    {
        savedSpeed = agent.speed;
        savedAcc = agent.acceleration;
        pauseCount = pauseTime;
    }

    // Update is called once per frame
    void Update () {
        agent.SetDestination(Target.position);
    }

    public void PauseMovement()
    {
        if (pauseCount >= pauseTime)
        {
            StartCoroutine(PauseEnemy());
        }
        else
        {
            pauseCount = 0;
        }
    }

    public void StopMovement()
    {
        agent.speed = 0;
        agent.acceleration = 100;
        pauseCount = 0;
    }

    public void ResumeMovement()
    {
        agent.speed = savedSpeed;
        agent.acceleration = savedAcc;
    }

    IEnumerator PauseEnemy()
    {
        StopMovement();
        while(pauseCount < pauseTime)
        {
            yield return null;
            pauseCount += Time.deltaTime;
        }
        ResumeMovement();
    }
}
