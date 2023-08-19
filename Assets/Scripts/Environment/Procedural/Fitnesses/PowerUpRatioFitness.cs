using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PowerUpRatioFitness : InLoopFitnessBase
{
    ArrayList areaSize;
    bool[,] ischecked;
    int powerUpAmount;
    [SerializeField] bool inRatioFormat;
    [Range(0.0f, 100.0f)]
    [SerializeField] float minPowerupAmount;
    [Range(0.0f, 100.0f)]
    [SerializeField] float maxPowerupAmount;

    public override void calculateFitness(int[,] map, Coordinate currCoor)
    {
        int size = 1, i = currCoor.yCoor, j = currCoor.xCoor;
        if (map[i, j] == 2)
            powerUpAmount++;
        if (map[i, j] == 1 || ischecked[i, j])
            return;

        ischecked[i, j] = true;
        Queue<Coordinate> q = new Queue<Coordinate>();
        Coordinate c, tempCoor;
        q.Enqueue(new Coordinate(j, i));
        //Ngambil Ukuran area 1 per 1
        while (q.Count > 0)
        {
            c = q.Dequeue();
            for (int k = 0; k < 4; k++)
            {
                tempCoor = new Coordinate(c.xCoor + Mathf.RoundToInt(Mathf.Sin(k * Mathf.PI / 2)), c.yCoor + Mathf.RoundToInt(Mathf.Cos(k * Mathf.PI / 2)));
                if (tempCoor.xCoor >= 0 && tempCoor.yCoor >= 0 && tempCoor.yCoor < SetObjects.getHeight() && tempCoor.xCoor < SetObjects.getWidth() && map[tempCoor.yCoor, tempCoor.xCoor] != 1 && !ischecked[tempCoor.yCoor, tempCoor.xCoor])
                {
                    ischecked[tempCoor.yCoor, tempCoor.xCoor] = true;
                    q.Enqueue(tempCoor);
                    size++;
                }
            }
        }
        areaSize.Add(size);
    }

    public override float getFitnessScore()
    {
        float biggest = -999;

        // Ini aku pakai area yang bisa diakses player, bukan panjang * lebar Arena
        for (int i = 0; i < areaSize.Count; i++)
        {
            if ((int)areaSize[i] > biggest)
                biggest = (int)areaSize[i];
        }

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
        areaSize = new ArrayList();
        ischecked = new bool[SetObjects.getHeight(), SetObjects.getWidth()];
        fitnessTotal = 0;
        powerUpAmount = 0;
    }

    public float getRatio()
    {
        return minPowerupAmount / 100;
    }

}
