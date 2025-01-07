using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AI_Brain : MonoBehaviour
{
    [Header("Health Settings")]
    private float maxHealth = 500.0f; // Maximum health of the enemy
    public float currentHealth;
    private float minHealth = 0;// Current health of the enemy
    public float healthRegenRate = 1.0f; // Health regenerated per second
    private float healthRegenTimer = 2.0f;
    
    public float minProjectileVelocity = 5.0f; // Adjust this value to define the minimum velocity required to register a hit
    
    [Header("Consideration Settings")]
    public float healthConsiderationScore;
    [SerializeField] private AnimationCurve healthResponseCurve;

    public float ceConsiderationScore;
    [SerializeField] private AnimationCurve ceResponseCurve;


    [Header("Cursed Technique Settings")]
    public float maxCursedEnergy = 100.0f; // Maximum health of the enemy
    public float currentCE; // Current health of the enemy
    private float minCursedEnergy = 0;
    public float CERegenRate = 1.0f; // Health regenerated per second
    private float CERegenTimer = 2.0f; 
    [HideInInspector]public int teamLayerIndex;
    
    public float attackRange = 1.5f;      // Adjust this value based on your attack range
    public float retreatDistance = 2.0f;  // Distance to retreat when the enemy gets too close
    public float strafeDuration = 2.0f;    // Duration for strafing
    public float strafeDistance = 1.0f;   // Distance to strafe from the target
    public bool isAttacking = false;
    
    public NavMeshAgent agent;
    public Transform enemyTarget;
    private float strafeTimer;

    private void Start()
    {
        currentHealth = maxHealth;
        currentCE = maxCursedEnergy;
        
        agent = GetComponent<NavMeshAgent>();
        strafeTimer = strafeDuration; // Start the timer at the duration to initiate strafing immediately
    }

    private void FixedUpdate()
    {
        #region Health Regen
        // Health regeneration timer
        if (currentHealth < maxHealth)
        {
            healthRegenTimer += Time.deltaTime;
            if (healthRegenTimer >= 1.0f / healthRegenRate)
            {
                healthRegenTimer = 0.0f;
                currentHealth = Mathf.Min(currentHealth + 1, maxHealth);
            }
        }
        #endregion
        
        #region CE Regen
        if (currentCE < maxCursedEnergy)
        {
            CERegenTimer += Time.deltaTime;
            if (CERegenTimer >= 1.0f / CERegenRate)
            {
                CERegenTimer = 0.0f;
                currentCE = Mathf.Min(currentCE + 1, maxCursedEnergy);
            }
        }
        #endregion

        #region Consideration Normalization
        // Normalize currentHealth between 0 and 1 using minHealth and maxHealth
        float normalizedHealth = Normalize(currentHealth, minHealth, maxHealth);
        float normalizedCE = Normalize(currentCE, minCursedEnergy, maxCursedEnergy);
        // Evaluate using AnimationCurves
        float playerHealthScore = healthResponseCurve.Evaluate(normalizedHealth);
        float playerCeScore = ceResponseCurve.Evaluate(normalizedCE);

        healthConsiderationScore = playerHealthScore;
        ceConsiderationScore = playerCeScore;

        #endregion
        
    }

    private float Normalize(float value, float minValue, float maxValue)
    {
        return Mathf.Clamp01((value - minValue) / (maxValue - minValue));
    }

    #region Damage Logic
    private void OnCollisionEnter(Collision collision)
    {

        // Check the velocity of the incoming projectile
            Rigidbody projectileRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (projectileRigidbody != null)
            {
                float projectileVelocity = projectileRigidbody.velocity.magnitude;
                if (projectileVelocity >= minProjectileVelocity)
                {
                    // Decrease the enemy's health
                    
                    TakeDamage(5.0f); // You can adjust the damage value as needed
                    
                }
            }

    }

    private void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        // Implement enemy death behavior here, e.g., play death animation, spawn items, etc.
        Destroy(gameObject); // For simplicity, destroy the enemy game object when health reaches 0.
    }
    

    #endregion

    #region Movement Logic
        
    

    #endregion
    
    
}