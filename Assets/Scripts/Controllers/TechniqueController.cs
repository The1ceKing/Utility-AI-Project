using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace CE_Core
{
    public class TechniqueController : MonoBehaviour
    {
        [SerializeField] private bool anyTechniqueIsActive;
        [Header("DEVELOPMENT ONLY")] public bool clearListsFlag = false;
        [Header("Known Techniques")] public List<CursedTechnique> knownAbilities = new List<CursedTechnique>();
        [Header(" Ready Techniques")] public List<CursedTechnique> readyAbilities = new List<CursedTechnique>();
        [Header("AI Logic")] public AI_Brain aiBrain;
        
        public float timeBetweenActions;

        private void Start()
        {
            readyAbilities.AddRange(knownAbilities);
            foreach (CursedTechnique technique in readyAbilities)
            {
                technique.techniqueLayerIndex = aiBrain.teamLayerIndex;
            }
        }

        private void Update()
        {
            CheckActiveAbilities();

            if (!anyTechniqueIsActive && readyAbilities.Count > 0)
            {
                if (!IsInvoking("UseAbilityWithHighestWeight"))
                {
                    Invoke("UseAbilityWithHighestWeight",Random.Range(0.25f, timeBetweenActions)); 
                }
            }

            ContinuousAbilityDrain();
        }

        

        #region Technique Handling
        void CheckActiveAbilities()
        {
            foreach (CursedTechnique technique in readyAbilities)
            {
                if (technique.isActive && technique.techniqueType != CursedTechnique.TechniqueType.Continuous)
                {
                    anyTechniqueIsActive = true;
                    break;
                }
                else
                {
                    anyTechniqueIsActive = false;
                }
            }
        }

        void UseAbilityWithHighestWeight()
        {
            if (readyAbilities.Count > 0 && !anyTechniqueIsActive)
            {
                CursedTechnique selectedAbility = null;
                float highestWeight = float.MinValue;

                foreach (CursedTechnique ability in readyAbilities)
                {
                    if (!ability.isOnCooldown && aiBrain.currentCE >= ability.CE_Cost && !ability.isActive)
                    {
                        float considerationScore = EvaluateConsiderationScore(ability.considerations);
                
                        
                        float weightedScore = (considerationScore + ability.Weight);

                        if (weightedScore > highestWeight)
                        {
                            highestWeight = weightedScore;
                            selectedAbility = ability;
                        }
                    }
                }

                if (selectedAbility != null)
                {
                    aiBrain.currentCE -= selectedAbility.CE_Cost;
                    selectedAbility.UseTechnique();
                }
            }
        }

        void ContinuousAbilityDrain()
        {
            foreach (CursedTechnique technique in readyAbilities)
            {
                if (technique.isActive && technique.techniqueType == CursedTechnique.TechniqueType.Continuous && aiBrain.currentCE > 0)
                {
                    float ceDrainAmount = Time.deltaTime * technique.CE_Cost;
                    aiBrain.currentCE -= ceDrainAmount;
                }
            }
        }
        #endregion

        #region Action Scoring
        float EvaluateConsiderationScore(CursedTechnique.Considerations consideration)
        {
            switch (consideration)
            {
                case CursedTechnique.Considerations.Health:
                    return EvaluateConsiderationScore(aiBrain.healthConsiderationScore);

                case CursedTechnique.Considerations.CursedEnergy:
                    return EvaluateConsiderationScore(aiBrain.ceConsiderationScore);

                default:
                    return 0f;
            }
        }

        float EvaluateConsiderationScore(float considerationScore)
        {
            // You can add any logic here to modify the consideration score if needed.
            return considerationScore;
        }
        #endregion

        private void ClearLists()
        {
            readyAbilities.Clear();
            Debug.Log("Lists Cleared");
            clearListsFlag = false;
        }
    }
}
