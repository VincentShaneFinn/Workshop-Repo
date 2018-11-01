using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementController : MonoBehaviour {

    private Transform Target;
    public bool UseDestination = false;
    private Vector3 Destination;
    public NavMeshAgent agent;
    private float savedSpeed;
    private float savedAcc;
    private float pauseTime = .3f;
    private float pauseCount;

    public LayerMask collisionLayer;

    void Start()
    {
        savedSpeed = agent.speed;
        savedAcc = agent.acceleration;
        pauseCount = pauseTime;
        Target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update () {
        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            if (UseDestination)
                agent.SetDestination(Destination);
            else
                agent.SetDestination(Target.position);
        }
    }

    public void SetTarget(Transform NewTarget)
    {
        Target = NewTarget;
        UseDestination = false;
    }
    public void SetDestination(Vector3 pos)
    {
        Destination = pos;
        UseDestination = true;
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

    public float GetRemainingDistance()
    {
        if (agent.isOnNavMesh)
            return agent.remainingDistance;
        return Mathf.Infinity;
    }

    public void StopMovement()
    {
        RaycastHit hit;

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 3, collisionLayer))
        {
            transform.position = new Vector3(hit.point.x, hit.point.y + agent.baseOffset, hit.point.z);
        }
        else if (Physics.Raycast(transform.position, Vector3.up, out hit, 3, collisionLayer))
        {
            transform.position = new Vector3(hit.point.x, hit.point.y + agent.baseOffset, hit.point.z);
        }

        if(agent.isOnNavMesh)
            agent.isStopped = true;

        agent.speed = 0;
        agent.acceleration = 100;


        //pauseCount = 0;
    }

    public void ResumeMovement()
    {
        RaycastHit hit;

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 20, collisionLayer))
        {
            transform.position = new Vector3(hit.point.x, hit.point.y + agent.baseOffset, hit.point.z);
        }
        else if (Physics.Raycast(transform.position, Vector3.up, out hit, 3, collisionLayer))
        {
            transform.position = new Vector3(hit.point.x, hit.point.y + agent.baseOffset, hit.point.z);
        }

        if (agent.isOnNavMesh)
            agent.isStopped = false;
        agent.speed = savedSpeed;
        agent.acceleration = savedAcc;
    }

    IEnumerator PauseEnemy()
    {
        if (GetComponent<EnemyAI>().CurrentStatus == EnemyBehaviorStatus.Staggered)
            yield break;
        GetComponent<EnemyAI>().ChangeStatus(EnemyBehaviorStatus.Staggered);

        while(pauseCount < pauseTime)
        {
            yield return null;
            pauseCount += Time.deltaTime;
        }
        GetComponent<EnemyAI>().ChangeStatus(EnemyBehaviorStatus.Waiting);
    }

    public void DisableNavAgent()
    {
        StopMovement();
        agent.enabled = false;
    }

    public void EnableNavAgent()
    {
        agent.enabled = true;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z); //MARK WILL NOT WORK IN BASEMENTS
        ResumeMovement();
    }

    public IEnumerator KnockbackEnemy()
    {
        if (agent.isActiveAndEnabled)
        {
            GetComponent<EnemyAI>().ChangeStatus(EnemyBehaviorStatus.Staggered);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            float time = .4f;//.15f;
            float speed = 7; // keep greater than 6
            Vector3 dir = (transform.position - player.transform.position).normalized;
            dir.y = 0;

            float count = 0;
            while (count < time)
            {
                float currentKnockbackSpeed = speed - savedSpeed;
                gameObject.transform.position += (dir * (savedSpeed + currentKnockbackSpeed * (1 - count / time)) * Time.deltaTime);
                yield return null;
                count += Time.deltaTime;

            }

            count = .2f;
            while(count < time)
            {
                yield return null;
                count += Time.deltaTime;
            }

            GetComponent<EnemyAI>().ChangeStatus(GetComponent<EnemyAI>().PreviousStatus); //return to what it was originally doing
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.name == "Sword")
        {
            StartCoroutine(KnockbackEnemy());
        }
    }
}
