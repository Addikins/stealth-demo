using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StealthGame.Movement;
using UnityEngine.AI;
using System;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float runSpeed = 6f;
    [SerializeField] float speedSmoothTime = 0.15f;
    [SerializeField] float walkingAnimationSpeed = 2f;
    [SerializeField] float runningAnimationSpeed = 3f;
    [SerializeField] Transform target;

    bool running = false;
    float currentSpeed;
    float speedSmoothVelocity;
    Vector2 direction;

    Mover mover;
    Animator animator;
    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        mover = GetComponent<Mover>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            MoveTowardsPlayer();
        }
        UpdateAnimation();
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
}
