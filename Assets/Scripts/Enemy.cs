using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    public Transform target;

    private StateMachine stateMachine;
    private Rigidbody rb;

    readonly float aggroRange = 100;
    readonly float deAggroDelay = 10;
    float lastSightingTime;
    readonly float attackRange = 1;
    readonly float attackDelay = 1;
    float attackTime;

    readonly float ragdollDelay = 5;
    float ragdollTime;

    readonly float getUpDelay = 2;
    quaternion getUpRotationStart;
    quaternion getUpRotationEnd = quaternion.identity;
    float getUpTime;

    public bool ragdoll = false;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        navMeshAgent.stoppingDistance = attackRange;

        stateMachine = new StateMachine();

        StateMachineState idleState = new StateMachineState();
        StateMachineState chaseState = new StateMachineState();
        StateMachineState attackState = new StateMachineState();
        StateMachineState ragdollState = new StateMachineState();
        StateMachineState getUpState = new StateMachineState();
        // StateMachineState deadState = new StateMachineState();

        idleState.transitions = new List<StateMachineState> { chaseState, ragdollState };
        chaseState.transitions = new List<StateMachineState> { attackState, idleState, ragdollState };
        attackState.transitions = new List<StateMachineState> { chaseState, ragdollState };
        ragdollState.transitions = new List<StateMachineState> { getUpState };
        getUpState.transitions = new List<StateMachineState> { idleState };

        idleState.Enter = () =>
        {
            navMeshAgent.isStopped = false;
        };

        idleState.Update = () =>
        {
            if (Physics.Raycast(transform.position, target.position - transform.position, out RaycastHit hit, aggroRange))
            {
                if (hit.transform == target)
                {
                    lastSightingTime = Time.time;
                    return 0;
                }
            }

            if (ragdoll)
            {
                return 1;
            }

            // random walk
            if (Random.value < 0.02)
            {
                Vector3 randomDirection = Random.insideUnitSphere * 10;
                randomDirection += transform.position;
                NavMeshHit hit2;
                NavMesh.SamplePosition(randomDirection, out hit2, 10, 1);
                Vector3 finalPosition = hit2.position;
                navMeshAgent.SetDestination(finalPosition);
            }


            return -1;
        };

        chaseState.Enter = () =>
        {
            navMeshAgent.isStopped = false;
        };

        chaseState.Update = () =>
        {
            if (Physics.Raycast(transform.position, target.position - transform.position, out RaycastHit hit, aggroRange))
            {
                if (hit.transform == target)
                {
                    navMeshAgent.SetDestination(target.position);
                    lastSightingTime = Time.time;
                    if (Vector3.Distance(transform.position, target.position) < attackRange)
                    {
                        return 0;
                    }
                    return -1;
                }
            }
            if (Time.time - lastSightingTime > deAggroDelay)
            {
                return 1;
            }

            if (ragdoll)
            {
                return 1;
            }

            return -1;
        };

        attackState.Enter = () =>
        {
            navMeshAgent.isStopped = true;
            attackTime = Time.time;
            Debug.Log("Attacking!");
        };

        attackState.Update = () =>
        {
            // TODO: do the attack thing


            if (Time.time - attackTime > attackDelay)
            {
                return 0;
            }

            if (ragdoll)
            {
                return 1;
            }

            return -1;
        };

        ragdollState.Enter = () =>
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.enabled = false;
            rb.constraints = RigidbodyConstraints.None;
            rb.angularVelocity = Random.insideUnitSphere * 2.0f;
            rb.velocity = Random.insideUnitSphere * 1.0f;
            Debug.Log("Ragdoll!");
        };

        ragdollState.Update = () =>
        {
            if (Time.time - ragdollTime > ragdollDelay)
            {
                return 0;
            }

            return -1;
        };

        ragdollState.Exit = () =>
        {
            ragdoll = false;
        };

        getUpState.Enter = () =>
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            getUpTime = Time.time;
            getUpRotationStart = transform.rotation;
            Debug.Log("Getting up!");
        };

        getUpState.Update = () =>
        {
            if (Time.time - getUpTime > getUpDelay)
            {
                return 0;
            }

            // interpolate rotation
            float t = (Time.time - getUpTime) / getUpDelay;
            transform.rotation = math.slerp(getUpRotationStart, getUpRotationEnd, t);

            return -1;
        };

        getUpState.Exit = () =>
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            navMeshAgent.enabled = true;

        };

        stateMachine.SetState(idleState);
    }


    // Update is called once per frame
    void Update()
    {
        stateMachine.Step();
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision!");
        if (collision.gameObject.CompareTag("Player") && !ragdoll)
        {
            ragdoll = true;
            ragdollTime = Time.time;
        }
    }
}
