using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementController : MonoBehaviour {

    private Transform Target;
    public NavMeshAgent agent;
    private float savedSpeed;
    private float savedAcc;
    private float pauseTime = 1;
    private float pauseCount;

    private bool isStaggered = false; //tempStatus for now

    void Start()
    {
        savedSpeed = agent.speed;
        savedAcc = agent.acceleration;
        pauseCount = pauseTime;
        Target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update () {
        agent.SetDestination(Target.position);
    }

    public void SetTarget(Transform NewTarget)
    {
        Target = NewTarget;
    }
    public Transform GetTarget() { return Target; }

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
        agent.isStopped = true;
        agent.speed = 0;
        agent.acceleration = 100;

        pauseCount = 0;
    }

    public void ResumeMovement()
    {
        agent.isStopped = false;
        agent.speed = savedSpeed;
        agent.acceleration = savedAcc;
    }

    IEnumerator PauseEnemy()
    {
        if (isStaggered)
            yield break;
        StopMovement();
        while(pauseCount < pauseTime)
        {
            yield return null;
            pauseCount += Time.deltaTime;
        }
        ResumeMovement();
    }

    public IEnumerator KnockbackEnemy()
    {
        isStaggered = true;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float time = .15f;
        float speed = 20; // keep greater than 6
        Vector3 dir = (transform.position - player.transform.position).normalized;

        float count = 0;
        while (count <= time)
        {
            yield return null;
            count += Time.deltaTime;
            float currentKnockbackSpeed = speed - savedSpeed;
            gameObject.transform.position += (dir * (savedSpeed + currentKnockbackSpeed * (1 - count / time)) * Time.deltaTime);

        }
        isStaggered = false;
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.name == "Sword")
        {
            StartCoroutine(KnockbackEnemy());
        }
    }
}
