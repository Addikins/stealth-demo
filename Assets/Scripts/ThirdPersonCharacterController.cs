using System;
using UnityEngine;

namespace RPG.Control {

    public class ThirdPersonCharacterController : MonoBehaviour {

        [SerializeField] float gravity = -10f;
        [SerializeField] float walkSpeed = 2f;
        [SerializeField] float runSpeed = 6f;
        [SerializeField] float speedSmoothTime = 0.15f;
        [SerializeField] float turnSmoothTime = 0.2f;
        [SerializeField] float walkingAnimationSpeed = .5f;
        [SerializeField] float runningAnimationSpeed = 1f;
        [SerializeField] float attackCooldown = .5f;
        [SerializeField] float attackTime = .5f;
        private State state;

        private enum State {
            Normal,
            Attacking,
        }

        private float defaultRunAnimation;
        private Vector3 velocity;
        private float velocityY;
        private float timeAttacking;
        private float timeSinceLastAttack;

        float turnSmoothVelocity;
        float speedSmoothVelocity;
        float currentSpeed;
        Transform cameraT;
        Animator animator;
        CharacterController controller;
        private float timeSinceLastInput;

        void Start () {
            state = State.Normal;
            animator = GetComponent<Animator> ();

            cameraT = Camera.main.transform;
            defaultRunAnimation = runningAnimationSpeed;
            currentSpeed = walkSpeed;

            // distanceToGround = GetComponent<SphereCollider> ().bounds.extents.y;
            controller = GetComponent<CharacterController> ();

        }

        void Update () {
            switch (state) {
                case State.Normal:
                    if (!Input.anyKeyDown) { timeSinceLastInput += Time.deltaTime; } else { timeSinceLastInput = 0; }
                    Movement ();
                    CheckAttack ();
                    break;
                case State.Attacking:
                    Attacking ();
                    break;
            }
        }

        private void Movement () {

            bool running = Input.GetKey (KeyCode.LeftShift);
            Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
            Vector2 inputDir = input.normalized;

            if (inputDir != Vector2.zero) {
                float targetRotation = Mathf.Atan2 (inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle (transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
            }
            float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
            currentSpeed = Mathf.SmoothDamp (currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

            HandleMovement ();

            if (controller.isGrounded) {
                velocityY = 0;
            }

            float animationSpeed = ((running) ? runningAnimationSpeed : walkingAnimationSpeed) * inputDir.magnitude;
            animator.SetFloat ("forwardSpeed", animationSpeed, speedSmoothTime, Time.deltaTime);
        }

        private void HandleMovement () {
            velocityY += gravity * Time.deltaTime;
            velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
            controller.Move (velocity * Time.deltaTime);
        }

        private void CheckAttack () {
            timeSinceLastAttack += Time.deltaTime;
            if (Input.GetMouseButtonDown (0) && timeSinceLastAttack > attackCooldown) {
                animator.SetTrigger ("attack");
                animator.SetFloat ("attackMotion", UnityEngine.Random.Range (0, 4));
                state = State.Attacking;
                timeSinceLastAttack = 0f;
                return;
            }
        }

        private void Attacking () {
            if (timeAttacking > attackTime) {
                timeAttacking = 0;
                state = State.Normal;
                print ("Stopping Attack");
            }
            timeAttacking += Time.deltaTime;
        }
    }
}