using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Teleport : CursedTechnique
{
    [Header("Teleportation Settings")]
    public Transform Target; // The Target that will be the anchor to teleport around.
    public float teleportRadius = 5.0f; // Maximum teleport radius
    
    private void Start()
    {
        //This ability will be able to be called even if another ability is active if there is an incoming projectile or enemy.
        
    }
    
    public override void UseTechnique()
    {
        isActive = true;
        teleportAroundTarget();
    }

    void teleportAroundTarget()
    {
        // Teleport to a random position within a point around the target
        Vector3 playerPos = Target.position;
        Vector2 randomCircle = Random.insideUnitCircle * teleportRadius;
        Vector3 teleportPos = new Vector3(playerPos.x + randomCircle.x, playerPos.y, playerPos.z + randomCircle.y);
        transform.position = teleportPos;
        isActive = false;
        StartCoroutine(TechniqueCooldown());
    }
    
    public override IEnumerator TechniqueCooldown()
    {
        isOnCooldown = true;
        
        yield return new WaitForSeconds(techniqueCooldown);
        
        isOnCooldown = false;
        
    }
}