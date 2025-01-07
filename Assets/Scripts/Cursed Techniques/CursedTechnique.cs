using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CursedTechnique : MonoBehaviour
{
    [Header("Technique Settings")] 
    public float Weight;
    public int CE_Cost; // How much does it cost to perform this technique.
    public bool isActive; // This is how I know which techniques are Active, two techniques should never be active at the same time unless one of them is continuous.
    public bool isOnCooldown; // This is a check if a ability need to be removed from being active.
    public TechniqueType techniqueType;
    public Considerations considerations;
    public int techniqueLayerIndex;
    public float techniqueCooldown = 3.0f; // The amount of time before the character can use this technique again.

    public enum TechniqueType
    {
        Normal,
        Continuous,
        Reactive,
        Stunning,
    }

    public enum Considerations
    {
        None,
        Health,
        CursedEnergy,
        EnemyProximity,
    }
    
    public abstract void UseTechnique();

    public abstract IEnumerator TechniqueCooldown();

}
