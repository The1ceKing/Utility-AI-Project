using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using CE_Core;

namespace CE_Core
{
    public class Attract : CursedTechnique
    {
        [Header("Projectile Logic")] public Transform target; // The target position to attract objects to.
        public float attractionRadius = 5f; // Radius within which objects are attracted.
        [Range(100f, 300f)] public float attractionForce = 10f; // Attraction force strength.
        [Range(100f, 300f)] public float pickupForce = 50f; // Force pushing the cubes off the ground.
        [Range(0f, 300f)] public float rotationTorque = 1f; // Torque to make attracted objects rotate.
        [Range(300f, 800f)] public float projectileForce = 50f; // Force applied to make objects a projectile.
        [Range(0f, 45f)] public float downwardAngle = 30f; // Angle at which projectiles are launched downward.
        [Range(1f, 10f)] public float attractionTime = 5.0f; // The Amount of time it will attract and hold objects.

        private List<Rigidbody> attractedObjects = new List<Rigidbody>();
        public int maxAttractedObject;
        public bool isAttracting;

        public Transform attackTarget;

        private bool hasThrown = false; // Flag to check if Throw() has been called.

        private void Start()
        {
            attractedObjects.Clear();
            
        }

        void Update()
        {
            if (isActive)
            {
                Attraction();
            }
            else
            {
                hasThrown = false;
            }

            if (isActive && isAttracting && !hasThrown)
            {
                // Ensure that useMeteorStrike coroutine is not already running.
                if (!IsInvoking("Throw"))
                {
                    Invoke("Throw", attractionTime); // Call Throw() after a delay.
                }
            }

        }

        public void Attraction()
        {
            isAttracting = true;
            

            // Find all colliders within the specified radius.
            Collider[] colliders = Physics.OverlapSphere(target.position, attractionRadius);

            foreach (Collider col in colliders)
            {
                if (col.CompareTag("Grabbable"))
                {
                    // Check if the collider has a Rigidbody component.
                    Rigidbody rb = col.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        //rb.gameObject.layer = (int)techniqueLayer;
                        //print(base.techniqueLayer);
                        attractedObjects.Add(rb);

                    }

                    if (attractedObjects.Count >= maxAttractedObject)
                    {
                        // We've reached the maximum attraction limit, exit the loop
                        break;
                    }
                }
            }

            foreach (Rigidbody rb in attractedObjects)
            {

                rb.gameObject.layer = techniqueLayerIndex;
                switch (techniqueType)
                {
                    case TechniqueType.Stunning:
                        rb.tag = "Stunning_Projectile";
                        break;
                    case TechniqueType.Normal:
                        rb.tag = "Normal_Projectile";
                        break;
                    case TechniqueType.Continuous:
                        Debug.Log("This ability shouldn't be continuous");
                        break;
                    case TechniqueType.Reactive:
                        Debug.Log("This ability shouldn't be continuous");
                        break;
                }

                // Calculate the direction from the object to the target.
                Vector3 direction = target.position - rb.transform.position;

                // Apply an initial force to pick up the object from the ground.
                rb.AddForce(Vector3.up * pickupForce);

                // Apply an attracting force to the Rigidbody.
                rb.AddForce(direction.normalized * attractionForce);

                // Apply torque to make the object rotate slightly.
                Vector3 randomTorque = Random.insideUnitSphere * rotationTorque;
                rb.AddTorque(randomTorque);
            }
        }

        public void Throw()
        {
            isAttracting = false;

            foreach (Rigidbody rb in attractedObjects)
            {
                // Calculate the direction from the object to the target.
                Vector3 fireDirection = attackTarget.position - rb.transform.position;

                // Apply an attracting force to the Rigidbody.
                ApplyForce(rb, fireDirection.normalized * projectileForce, ForceMode.Impulse);

                // Apply torque to make the object rotate slightly.
                ApplyTorque(rb);

                // Apply a downward angle to the projectile.
                Vector3 downwardForce =
                    Quaternion.Euler(downwardAngle, 0, 0) * fireDirection.normalized * projectileForce;
                ApplyForce(rb, downwardForce);

                StartCoroutine(TechniqueCooldown());
                StartCoroutine(ChangeTagAfterDelay(rb, 3f));
            }

            attractedObjects.Clear();
            isActive = false;
            hasThrown = true; // Set the flag to true to prevent further calls to Throw().
        }


        private void ApplyForce(Rigidbody rb, Vector3 force, ForceMode mode = ForceMode.Force)
        {
            rb.AddForce(force, mode);
        }

        private void ApplyTorque(Rigidbody rb)
        {
            Vector3 randomTorque = Random.insideUnitSphere * rotationTorque;
            rb.AddTorque(randomTorque);
        }

        public override void UseTechnique()
        {
            isActive = true;
        }

        private IEnumerator ChangeTagAfterDelay(Rigidbody rb, float delay)
        {
            yield return new WaitForSeconds(delay);
            rb.tag = "Grabbable";
        }

        public override IEnumerator TechniqueCooldown()
        {
            isOnCooldown = true;

            yield return new WaitForSeconds(techniqueCooldown);

            isOnCooldown = false;

        }

    }
}
