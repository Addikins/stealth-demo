using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StealthGame.Movement
{

    public class Mover : MonoBehaviour
    {
        [SerializeField] float gravity = -10f;
        [SerializeField] float sneakSpeed = 1f;
        [SerializeField] float walkSpeed = 2f;
        [SerializeField] float runSpeed = 6f;
        [SerializeField] float speedSmoothTime = 0.15f;
        [SerializeField] float sneakingAnimationSpeed = 1f;
        [SerializeField] float walkingAnimationSpeed = 2f;
        [SerializeField] float runningAnimationSpeed = 3f;

        Vector3 velocity;
        float velocityY;
        float timeAttacking;
        float timeSinceLastAttack;
        float currentSpeed;
        float speedSmoothVelocity;
        bool running = false;
        bool sneaking = false;

        Animator animator;
        CharacterController controller;

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            controller = GetComponent<CharacterController>();
        }

        public void HandleMovement(Vector2 direction)
        {
            CheckSpeedAlteration();

            float targetSpeed = (running ? runSpeed : sneaking ? sneakSpeed : walkSpeed) * direction.magnitude;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

            velocityY += gravity * Time.deltaTime;
            velocity = transform.forward * currentSpeed + Vector3.up * velocityY;

            controller.Move(velocity * Time.deltaTime);
            if (controller.isGrounded)
            {
                velocityY = 0;
            }

            float animationSpeed = (running ? runningAnimationSpeed : sneaking ? sneakingAnimationSpeed : walkingAnimationSpeed) * direction.magnitude;
            animator.SetFloat("forwardSpeed", animationSpeed, speedSmoothTime, Time.deltaTime);
        }

        private void CheckSpeedAlteration()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                running = true;
                sneaking = false;
            }
            else if (Input.GetMouseButton(1))
            {
                running = false;
                sneaking = true;
            }
            else { running = false; sneaking = false; }
        }
    }
}
