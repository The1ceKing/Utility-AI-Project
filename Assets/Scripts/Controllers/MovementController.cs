using UnityEngine;
using UnityEngine.AI;

public class MovementController : MonoBehaviour
{
    public float attackRange = 1.5f; // Adjust this value based on your attack range
    public float retreatDistance = 2.0f; // Distance to retreat when the enemy gets too close
    public float strafeDuration = 2.0f; // Duration for strafing
    public float strafeDistance = 1.0f;
    public float strafeOffset = 0.5f;   // Distance to strafe from the target
    public bool isAttacking = false;

    private NavMeshAgent navMeshAgent;
    public Transform enemyTarget;
    private bool inAttackRange = false;
    private float strafeTimer;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        strafeTimer = strafeDuration; // Start the timer at the duration to initiate strafing immediately
    }

    void Update()
    {
        // Assume you have a method to detect the enemy target
        DetectEnemy();

        // Assume you have a method to detect the enemy target
        DetectEnemy();

        if (enemyTarget != null)
        {
            // Check the distance to the enemy
            float distanceToEnemy = Vector3.Distance(transform.position, enemyTarget.position);

            if (distanceToEnemy > attackRange)
            {
                // If outside attack range, move towards the enemy
                navMeshAgent.SetDestination(enemyTarget.position);
                isAttacking = false;
                inAttackRange = false;
                strafeTimer = strafeDuration; // Reset the timer when not in attack range
            }
            else if (distanceToEnemy < retreatDistance)
            {
                // If too close, move away from the enemy
                Vector3 retreatDirection = transform.position - enemyTarget.position;
                Vector3 newDestination = transform.position + retreatDirection.normalized * retreatDistance;
                navMeshAgent.SetDestination(newDestination);
                isAttacking = false;
                inAttackRange = false;
                strafeTimer = strafeDuration; // Reset the timer when retreating
            }
            else
            {
                // If within attack range, set attack boolean to true
                isAttacking = true;

                // If not already in attack range, set the flag and wait for a brief moment before starting to strafe
                if (!inAttackRange)
                {
                    inAttackRange = true;
                    strafeTimer = strafeDuration; // Reset the timer when entering attack range
                }

                // Strafe left and right randomly with an offset after being in attack range for a brief moment
                strafeTimer -= Time.deltaTime;
                if (strafeTimer <= 0)
                {
                    // Randomly determine left or right strafe
                    float strafeDirection = Random.Range(0f, 1f) < 0.5f ? -1f : 1f;
                    Vector3 strafeOffsetVector = Quaternion.Euler(0, 90 * strafeDirection, 0) *
                                                 (transform.forward * strafeOffset);
                    Vector3 strafeDestination = enemyTarget.position + strafeOffsetVector +
                                                (transform.right * strafeDirection * strafeDistance);
                    navMeshAgent.SetDestination(strafeDestination);

                    // Reset the timer for the next strafe
                    strafeTimer = strafeDuration;
                }
            }
        }
        else
        {
            // No enemy detected, stop moving and reset attack boolean
            navMeshAgent.ResetPath();
            isAttacking = false;
            inAttackRange = false;
            strafeTimer = strafeDuration; // Reset the timer when no enemy is detected
        }

        void DetectEnemy()
        {
            // Implement your method to detect the enemy and set the enemyTarget variable
            // For example, you can use raycasting, colliders, or other detection methods
        }
    }
}
