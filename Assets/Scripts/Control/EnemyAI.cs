using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StealthGame.Movement;
using UnityEngine.AI;
using System;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Data")]
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float runSpeed = 6f;
    [SerializeField] float speedSmoothTime = 0.15f;
    [SerializeField] float walkingAnimationSpeed = 2f;
    [SerializeField] float runningAnimationSpeed = 3f;

    [Header("Chasing State Data")]
    [SerializeField] Color chaseSphereColor;
    [SerializeField] float chaseSphereRadius;
    [SerializeField] float chaseDistance = 15f;
    [SerializeField] float chaseTime = 5f;

    bool running = false;
    float currentSpeed;
    float speedSmoothVelocity;
    Vector2 direction;
    float timeSinceChasing = 0;

    Mover mover;
    Animator animator;
    NavMeshAgent agent;
    Transform target;

    private State state;

    private enum State
    {
        Normal,
        Chasing,
    }

    // Start is called before the first frame update
    void Start()
    {
        state = State.Normal;
        mover = GetComponent<Mover>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Normal:
                break;
            case State.Chasing:
                if (PlayerInRange() && timeSinceChasing < chaseTime)
                {
                    timeSinceChasing += Time.deltaTime;
                    MoveTowardsPlayer();
                }
                break;
        }
        UpdateAnimation();
    }

    private bool PlayerInRange()
    {
        return Vector3.Distance(target.position, transform.position) < chaseDistance;
    }

    private void MoveTowardsPlayer()
    {
        //transform.LookAt(target);
        direction = target.position - transform.position;
        //float targetSpeed = (running ? runSpeed : walkSpeed) * direction.magnitude;
        //agent.speed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);
        agent.speed = walkSpeed;
        agent.destination = target.position;
    }

    private void UpdateAnimation()
    {
        float animationSpeed = (running ? runningAnimationSpeed : walkingAnimationSpeed) * direction.magnitude;
        animator.SetFloat("forwardSpeed", animationSpeed, speedSmoothTime, Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            target = other.transform;
            state = State.Chasing;
        }
    }

    private void OnDrawGizmosSelected()
    {
        chaseSphereRadius = chaseDistance;
        Gizmos.color = chaseSphereColor;
        Gizmos.DrawWireSphere(transform.position + transform.forward * (chaseSphereRadius/2), chaseSphereRadius);
    }
}
