using UnityEngine;
using StealthGame.Movement;
using UnityEngine.AI;

namespace StealthGame.Control
{

    public class EnemyAI : MonoBehaviour
    {
        [Header("Movement Data")]
        [SerializeField] float walkSpeed = 2f;
        [SerializeField] float runSpeed = 6f;
        [SerializeField] float speedSmoothTime = 0.15f;
        [SerializeField] float walkingAnimationSpeed = 2f;
        [SerializeField] float runningAnimationSpeed = 3f;

        [Header("Chasing Data")]
        [SerializeField] Color chaseSphereColor;
        [SerializeField] float chaseSphereRadius;
        [SerializeField] float chaseDistance = 15f;
        [SerializeField] float chaseTime = 5f;
        [SerializeField] float suspectTime = 4f;

        [Header("Patrol Data")]
        [SerializeField] Vector3 startPoint;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float idleTime = 2f;
        [SerializeField] float waypointTolerance = 1f;

        [Header("Visual Indicators")]
        [SerializeField] Light spotlight;
        [SerializeField] Color dangerColor;
        [SerializeField] Color warningColor;


        bool running = false;
        bool walking = false;
        float speedSmoothVelocity;
        float direction;
        Vector3 lastKnownPosition;
        int currentWaypointIndex = 0;

        float timeSinceChasing = 0;
        float timeSuspecting = 0;
        float timeSinceArrivedAtWaypoint = 0;

        Mover mover;
        Animator animator;
        NavMeshAgent agent;
        Transform target;

        private State state;

        private enum State
        {
            Normal,
            Chasing,
            Suspecting,
        }

        // Start is called before the first frame update
        void Start()
        {
            state = State.Normal;
            mover = GetComponent<Mover>();
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();

            spotlight.gameObject.SetActive(false);
            startPoint = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case State.Normal:
                    PatrolBehavior();
                    timeSinceArrivedAtWaypoint += Time.deltaTime;
                    break;
                case State.Chasing:
                    if (target == null)
                    {
                        walking = true;
                        spotlight.color = warningColor;
                        state = State.Suspecting;
                        return;
                    }
                    else if (PlayerInRange() || timeSinceChasing <= chaseTime)
                    {
                        timeSinceChasing += Time.deltaTime;
                        lastKnownPosition = target.position;
                        MoveTo(target.position, runSpeed);
                        running = true;
                    }
                    else
                    {
                        target = null;
                        timeSinceChasing = 0;
                    }
                    break;
                case State.Suspecting:
                    running = false;
                    if (Vector3.Distance(lastKnownPosition, transform.position) > 8f)
                    {
                        MoveTo(lastKnownPosition, walkSpeed);
                    }
                    else
                    {
                        timeSuspecting += Time.deltaTime;
                        if (timeSuspecting >= suspectTime)
                        {
                            spotlight.gameObject.SetActive(false);
                            state = State.Normal;
                            return;
                        }
                    }
                    break;
            }
            UpdateAnimation();
        }

        private bool PlayerInRange()
        {
            return Vector3.Distance(target.position, transform.position) <= chaseDistance;
        }

        private void MoveTo(Vector3 destination, float speed)
        {
            direction = Vector3.Distance(destination, transform.position);
            agent.speed = speed;
            agent.destination = destination;
        }

        private void UpdateAnimation()
        {
            float animationSpeed = (running ? runningAnimationSpeed : walking ? walkingAnimationSpeed : 0);
            animator.SetFloat("forwardSpeed", animationSpeed, speedSmoothTime, Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                target = other.transform;
                spotlight.gameObject.SetActive(true);
                spotlight.color = dangerColor;
                state = State.Chasing;
            }
        }

        private void OnDrawGizmosSelected()
        {
            chaseSphereRadius = chaseDistance;
            Gizmos.color = chaseSphereColor;
            Gizmos.DrawWireSphere(transform.position + transform.forward * (chaseSphereRadius / 2), chaseSphereRadius);
        }

        private void PatrolBehavior()
        {
            Vector3 nextPosition = startPoint;

            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    walking = false;
                    timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }
            if (timeSinceArrivedAtWaypoint > idleTime)
            {
                walking = true;
                MoveTo(nextPosition, walkSpeed);
            }
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }
    }
}
