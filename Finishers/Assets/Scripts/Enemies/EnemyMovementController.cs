using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementController : MonoBehaviour {

    private Transform Target;
    public bool UseDestination = false;
    private Vector3 Destination;
    public NavMeshAgent agent;
    private bool LockToGround;
    private float savedSpeed;
    private float savedAcc;
    private float pauseTime = .3f;
    private float pauseCount;
    private bool AlreadyStaggered = false;

    public LayerMask collisionLayer;

    void Start()
    {
        savedSpeed = agent.speed;
        savedAcc = agent.acceleration;
        pauseCount = pauseTime;
        Target = GameObject.FindGameObjectWithTag("Player").transform;
        LockToGround = true;
    }

    // Update is called once per frame
    void Update () {
        RaycastHit hit;

        // Does the ray intersect any objects excluding the player layer
        if (LockToGround)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 3, collisionLayer))
            {
                transform.position = new Vector3(hit.point.x, hit.point.y + agent.baseOffset, hit.point.z);
            }
            else if (Physics.Raycast(transform.position, Vector3.up, out hit, 3, collisionLayer))
            {
                transform.position = new Vector3(hit.point.x, hit.point.y + agent.baseOffset, hit.point.z);
            }
        }

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

    //public void PauseMovement()
    //{
    //    if (pauseCount >= pauseTime)
    //    {
    //        StartCoroutine(PauseEnemy());
    //    }
    //    else
    //    {
    //        pauseCount = 0;
    //    }
    //}

    public float GetRemainingDistance()
    {
        if (agent.isOnNavMesh)
            return agent.remainingDistance;
        return Mathf.Infinity;
    }

    public void SetSpeed(float newSpeed)
    {
        agent.speed = newSpeed;
    }
    public void RestoreSpeed()
    {
        agent.speed = savedSpeed;
    }

    public void SetLockToGround(bool b)
    {
        LockToGround = b;
    }

    public void StopMovement()
    {
        //RaycastHit hit;

        //// Does the ray intersect any objects excluding the player layer
        //if (Physics.Raycast(transform.position, Vector3.down, out hit, 3, collisionLayer))
        //{
        //    transform.position = new Vector3(hit.point.x, hit.point.y + agent.baseOffset, hit.point.z);
        //}
        //else if (Physics.Raycast(transform.position, Vector3.up, out hit, 3, collisionLayer))
        //{
        //    transform.position = new Vector3(hit.point.x, hit.point.y + agent.baseOffset, hit.point.z);
        //}

        if(agent.isOnNavMesh)
            agent.isStopped = true;

        agent.speed = 0;
        agent.acceleration = 100;


        //pauseCount = 0;
    }

    public void ResumeMovement()
    {
        //RaycastHit hit;

        //// Does the ray intersect any objects excluding the player layer
        //if (Physics.Raycast(transform.position, Vector3.down, out hit, 20, collisionLayer))
        //{
        //    transform.position = new Vector3(hit.point.x, hit.point.y + agent.baseOffset, hit.point.z);
        //}
        //else if (Physics.Raycast(transform.position, Vector3.up, out hit, 3, collisionLayer))
        //{
        //    transform.position = new Vector3(hit.point.x, hit.point.y + agent.baseOffset, hit.point.z);
        //}

        if (agent.isOnNavMesh)
            agent.isStopped = false;
        agent.speed = savedSpeed;
        agent.acceleration = savedAcc;
    }

    //IEnumerator PauseEnemy()
    //{
    //    GetComponent<EnemyAI>().ChangeStatus(EnemyBehaviorStatus.Staggered);

    //    while(pauseCount < pauseTime)
    //    {
    //        yield return null;
    //        pauseCount += Time.deltaTime;
    //    }
    //    GetComponent<EnemyAI>().ChangeStatus(EnemyBehaviorStatus.Waiting);
    //}

    public void DisableNavAgent()
    {
        StopMovement();
        agent.enabled = false;
    }

    public void EnableNavAgent()
    {
        agent.enabled = true;
        ResumeMovement();
    }

    public void HelpKnockback(float optionalFactor = 7f)
    {
        print(optionalFactor);
        StartCoroutine(KnockbackEnemy(optionalFactor));
    }

    //currently a fundamental issue with this, we need to handle this a better way, basically restart if this coroutine is in progress
    private IEnumerator KnockbackEnemy(float speed)
    {
        if (agent.isActiveAndEnabled)
        {
            GetComponent<EnemyAI>().ResetStaggeredCheck();

            GetComponent<EnemyAI>().anim.Play("Hit4");
            GetComponent<EnemyAI>().ChangeStatus(EnemyBehaviorStatus.Staggered);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            float time = .3f;//.15f;
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
            float pcount = .2f;//pause for a second
            while (pcount < time)
            {
                yield return null;
                pcount += Time.deltaTime;
            }
            //this stuff doesn't work great just use an outside timer to restore status to waiting
            //if (GetComponent<EnemyAI>().PreviousStatus != EnemyBehaviorStatus.Staggered)
            //{
            //    GetComponent<EnemyAI>().ChangeStatus(GetComponent<EnemyAI>().PreviousStatus); //return to what it was originally doing
            //}
            //else
            //{
            //    GetComponent<EnemyAI>().ChangeStatus(EnemyBehaviorStatus.SurroundPlayer);
            //}
        }

    }
}
