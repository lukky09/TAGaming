using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PowerUpRatioFitness : InLoopFitnessBase
{
    int powerUpAmount;
    [SerializeField] bool inRatioFormat;
    [Range(0.0f, 100.0f)]
    [SerializeField] float minPowerupAmount;
    [Range(0.0f, 100.0f)]
    [SerializeField] float maxPowerupAmount;

    public override void calculateFitness(int[,] map, Coordinate currCoor)
    {
        int i = currCoor.yCoor, j = currCoor.xCoor;
        if (map[i, j] == 2)
            powerUpAmount++;
    }

    public override float getFitnessScore()
    {
        int minRatio, maxRatio;
        if (inRatioFormat)
        {
            minRatio = Mathf.RoundToInt(SetObjects.getHeight() * SetObjects.getWidth() * minPowerupAmount / 100);
            maxRatio = Mathf.RoundToInt(SetObjects.getHeight() * SetObjects.getWidth() * maxPowerupAmount / 100);
        }
        else
        {
            minRatio = Mathf.FloorToInt(minPowerupAmount);
            maxRatio = Mathf.FloorToInt(maxPowerupAmount);
        }
        if (maxRatio < minRatio)
        {
            Debug.LogWarning("Min dan max ditukar");
            (minRatio, maxRatio) = (minRatio, maxRatio);
        }

        float nilaiMinus = 0;
        if (powerUpAmount < minRatio)
            nilaiMinus = minRatio - powerUpAmount;
        else if (powerUpAmount > maxRatio)
            nilaiMinus = powerUpAmount - maxRatio;

        float nilaiMinusMax = SetObjects.getWidth() * SetObjects.getHeight() - maxRatio > minRatio ? SetObjects.getWidth() * SetObjects.getHeight() - maxRatio : minRatio;
        nilaiMinus = nilaiMinus / nilaiMinusMax;

        return MathF.Pow(1 - nilaiMinus, 2) * weight;
    }

    public override void resetVariables()
    {
        fitnessTotal = 0;
        powerUpAmount = 0;
    }

    public float getRatio()
    {
        return minPowerupAmount / 100;
    }

}
