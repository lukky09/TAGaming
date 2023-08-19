using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class RockAmountFitness : InLoopFitnessBase
{
    [Range(0.0f, 100.0f)]
    [SerializeField] float minRockRatio;
    [Range(0.0f, 100.0f)]
    [SerializeField] float maxRockRatio;
    int rockAmount;


    public override void calculateFitness(int[,] map, Coordinate currCoor)
    {
        if (map[currCoor.yCoor,currCoor.xCoor] == 1) 
            rockAmount++;
        
    }

    public override float getFitnessScore()
    {
        int maxRock = Mathf.RoundToInt(SetObjects.getWidth() * SetObjects.getHeight() * maxRockRatio / 100);
        int minrock = Mathf.RoundToInt(SetObjects.getWidth() * SetObjects.getHeight() * minRockRatio / 100);
        if (maxRock < minrock)
        {
            Debug.LogWarning("Min dan max ditukar");
            (minrock, maxRock) = (maxRock, minrock);
        }

        float nilaiMinus = 0;
        if (rockAmount < minrock)
            nilaiMinus = minrock - rockAmount;
        else if (rockAmount > maxRock)
            nilaiMinus = rockAmount - maxRock;

        //ini untuk batas normalisasi
        float nilaiMinusMax = SetObjects.getWidth() * SetObjects.getHeight() - maxRock > minrock ? SetObjects.getWidth() * SetObjects.getHeight() - maxRock : minrock;

        nilaiMinus /= nilaiMinusMax;
        float score = 1 - nilaiMinus;
        return Mathf.Pow(score, 2) * weight;
    }

    public override void resetVariables()
    {
        rockAmount = 0;
    }
}
