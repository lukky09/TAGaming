using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InLoopFitnessBase : FitnessBase
{
    protected float fitnessTotal;
    public abstract void calculateFitness(int[,] map, Coordinate currCoor);
}
