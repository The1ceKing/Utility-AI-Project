using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limitless : CursedTechnique
{
    
    public float barrierRadius = 3.0f;
    public float resetDelay = 0.5f;
    public Collider limitlessRange;
    
    public float ceDrainTimer = 2.0f;


    private void Awake()
    {
        isActive = false;
        
    }

    public override void UseTechnique()
    {
        isActive = !isActive;
    }

    public override IEnumerator TechniqueCooldown()
    {
        isOnCooldown = true;
        
        yield return new WaitForSeconds(techniqueCooldown);
        
        isOnCooldown = false;
        
    }

    #region Limitless Logic
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != techniqueLayerIndex && isActive)
        {
            
            if (limitlessRange.bounds.Contains(other.transform.position))
            {
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    //rb.isKinematic = true;

                    StartCoroutine(ResetPhysics(rb));
                }
            }
        }
       
    }
    
    private IEnumerator ResetPhysics(Rigidbody rb)
    {
        yield return new WaitForSeconds(resetDelay);

        
        // Re-enable physics properties (e.g., allow the object to move again)
        //rb.isKinematic = false;
        rb.WakeUp();
    }

    private void OnTriggerExit(Collider other)
    {
        if (limitlessRange.bounds.Contains(other.transform.position))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }
    }
    #endregion
    
    
    
}