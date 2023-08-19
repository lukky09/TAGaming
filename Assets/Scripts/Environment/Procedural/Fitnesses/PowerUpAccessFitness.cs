using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpAccessFitness : InLoopFitnessBase
{
    int[,] map;
    ArrayList lokasiPlayer;
    ArrayList lokasiPowerUp;

    public override void calculateFitness(int[,] map, Coordinate currCoor)
    {
        this.map = map;
        int i = currCoor.yCoor, j = currCoor.xCoor;
        if (map[i, j] == 2)
            lokasiPowerUp.Add(new Coordinate(j, i));
        if (map[i, j] == 3)
            lokasiPlayer.Add(new Coordinate(j, i));

    }

    public override float getFitnessScore()
    {
        int indexPlayer = 0;
        float tempDistance, biggest;
        if (lokasiPlayer.Count <= 0)
            return 0;

        for (int i = 0; i < lokasiPowerUp.Count; i++)
        {
            biggest = 999;
            //Ambil player terdekat biar Astar tidak terlalu lama
            for (int j = 0; j < lokasiPlayer.Count; j++)
            {
                tempDistance = Coordinate.Distance((Coordinate)lokasiPowerUp[i], (Coordinate)lokasiPlayer[j]);
                if (tempDistance < biggest)
                {
                    biggest = tempDistance;
                    indexPlayer = j;
                }
            }
            if (AStarAlgorithm.doAstarAlgo((Coordinate)lokasiPowerUp[i], (Coordinate)lokasiPlayer[indexPlayer], map) != null)
            {
                fitnessTotal++;
            }
        }
        if (lokasiPowerUp.Count > 0)
            return Mathf.Pow(fitnessTotal / lokasiPowerUp.Count, 2) * weight;
        else
            return 0;

    }    

    public override void resetVariables()
    {
        fitnessTotal = 0;
        lokasiPlayer = new ArrayList();
        lokasiPowerUp = new ArrayList();
    }
}
