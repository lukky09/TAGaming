using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FitnessBase : MonoBehaviour
{
    public string FitnessName { get { return fitnessName; } }
    public bool ForTemplateGen { get { return forTemplateGen; } }
    public bool ForTileGen { get { return forTileGen; } }
    public bool IsUsed { get { return isUsed; } }

    [SerializeField] bool isUsed = true;
    [SerializeField] bool forTileGen;
    [SerializeField] bool forTemplateGen;
    [SerializeField] string fitnessName;
    [SerializeField] protected float weight = 1;

    public abstract float getFitnessScore();

    public abstract void resetVariables();

}
