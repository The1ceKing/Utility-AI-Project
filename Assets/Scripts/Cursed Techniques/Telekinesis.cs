using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Telekinesis : CursedTechnique
{
    public Transform target; // The target position to attract objects to.
    public float attractionRadius = 5f; // Radius within which objects are attracted.
    [Range(100f, 300f)] public float attractionForce = 10f; // Attraction force strength.
    [Range(0f, 200f)]public float pickupForce = 50f; // Force pushing the cubes off the ground.
    [Range(100f, 300f)]public float rotationTorque = 1f; // Torque to make attracted objects rotate.
    [Range(100f, 500f)]public float projectileForce = 50f; // Force applied to make objects a projectile.
    [Range(1f, 10f)]public float attractionTime = 5.0f; // The Amount of time it will attract and hold objects.
    private bool isAttracting;
    private bool hasAttracted = false; // Flag to check if an object has been attracted.
    private Rigidbody attractedObject;
    public Transform attackTarget;
    private bool hasThrown = false; // Flag to check if Throw() has been called.

    void FixedUpdate()
    {
        if (isActive)
        {
            Attraction();
        }
        else
        {
            hasThrown = false;
            isAttracting = false;
            hasAttracted = false; // Reset the flag when not active.
        }

        if (attractedObject != null && !hasThrown)
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
                if (rb != null && attractedObject == null) // Check if no object has been attracted yet.
                {
                    attractedObject = rb;
                    attractedObject.gameObject.layer = techniqueLayerIndex;

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

                    break; // Exit the loop once one object has been attracted.
                }
            }
        }

        if (attractedObject != null)
        {
            // Calculate the direction from the object to the target.
            Vector3 direction = target.position - attractedObject.transform.position;

            // Apply an initial force to pick up the object from the ground.
            attractedObject.AddForce(Vector3.up * pickupForce);

            // Apply an attracting force to the Rigidbody.
            attractedObject.AddForce(direction.normalized * attractionForce);

            // Apply torque to make the object rotate slightly.
            Vector3 randomTorque = Random.insideUnitSphere * rotationTorque;
            attractedObject.AddTorque(randomTorque);
        }
    }

    public void Throw()
    {
        isAttracting = false;

        if (attractedObject != null)
        {
            // Calculate the direction from the object to the target.
            Vector3 fireDirection = attackTarget.position - attractedObject.transform.position;

            // Apply an attracting force to the Rigidbody.
            attractedObject.AddForce(fireDirection.normalized * projectileForce, ForceMode.Impulse);

            // Apply torque to make the object rotate slightly.
            Vector3 randomTorque = Random.insideUnitSphere * rotationTorque;
            attractedObject.AddTorque(randomTorque);

            StartCoroutine(TechniqueCooldown());
            StartCoroutine(ChangeTagAfterDelay(attractedObject, 1.5f));

            attractedObject = null;
            isActive = false;
            hasThrown = true; // Set the flag to true to prevent further calls to Throw().
        }
    }

    public override void UseTechnique()
    {
        isActive = true;
    }

    public override IEnumerator TechniqueCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(techniqueCooldown);

        isOnCooldown = false;
    }

    private IEnumerator ChangeTagAfterDelay(Rigidbody rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.tag = "Grabbable";
        rb.gameObject.layer = 0;
    }

    private void OnApplicationQuit()
    {
        // This method will be called when exiting play mode
        OnPlayModeExit();
    }

    private void OnPlayModeExit()
    {
        // Your code to execute when exiting play mode
        attractedObject = null;
        Debug.Log("Exiting play mode. Your method was called!");
    }
}
