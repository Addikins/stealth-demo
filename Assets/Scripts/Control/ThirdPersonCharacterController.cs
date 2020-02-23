﻿using System;
using UnityEngine;
using StealthGame.Movement;

namespace StealthGame.Control
{

    public class ThirdPersonCharacterController : MonoBehaviour
    {
        //[SerializeField] float attackCooldown = .5f;
        //[SerializeField] float attackTime = .5f;
        [SerializeField] float turnSmoothTime = 0.2f;

        float turnSmoothVelocity;
        float timeSinceLastInput;

        Transform cameraT;
        Mover mover;

        private State state;

        private enum State
        {
            Normal,
            Attacking,
        }

        void Start()
        {
            state = State.Normal;
            mover = GetComponent<Mover>();
            cameraT = Camera.main.transform;
        }

        void Update()
        {
            switch (state)
            {
                case State.Normal:
                    if (!Input.anyKeyDown) { timeSinceLastInput += Time.deltaTime; } else { timeSinceLastInput = 0; }
                    HandleMovement();
                    // CheckAttack ();
                    break;
                case State.Attacking:
                    // Attacking ();
                    break;
            }
        }

        private void HandleMovement()
        {

            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector2 inputDir = input.normalized;

            if (inputDir != Vector2.zero)
            {
                float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
            }

            mover.HandleMovement(inputDir);
        }

        // private void CheckAttack () {
        //     timeSinceLastAttack += Time.deltaTime;
        //     if (Input.GetMouseButtonDown (0) && timeSinceLastAttack > attackCooldown) {
        //         animator.SetTrigger ("attack");
        //         animator.SetFloat ("attackMotion", UnityEngine.Random.Range (0, 4));
        //         state = State.Attacking;
        //         timeSinceLastAttack = 0f;
        //         return;
        //     }
        // }

        // private void Attacking () {
        //     if (timeAttacking > attackTime) {
        //         timeAttacking = 0;
        //         state = State.Normal;
        //         print ("Stopping Attack");
        //     }
        //     timeAttacking += Time.deltaTime;
        // }
    }
}