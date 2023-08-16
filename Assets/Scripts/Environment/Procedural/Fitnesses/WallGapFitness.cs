using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGapFitness : InLoopFitnessBase
{

    [SerializeField] int WallGapLength;
    [SerializeField] int WallGapLengthMax;
    int jumlahGap;

    public override void calculateFitness(int[,] map, Coordinate currCoor)
    {
        int i = currCoor.yCoor, j = currCoor.xCoor;
        int jtemp, itemp;

        if (map[i, j] != 1)
            return;

        //Cek Horizontal
        if (j + 1 < SetObjects.getWidth() && map[i, j + 1] != 1)
        {
            jtemp = j + 1;
            while (jtemp < SetObjects.getWidth() && map[i, jtemp] != 1)
                jtemp++;
            if (jtemp - j < WallGapLengthMax)
            {
                jumlahGap++;
                fitnessTotal += Mathf.Log10((jtemp - j) * 10 / WallGapLength);
            }
        }
        //Cek Vertikal
        if (i + 1 < SetObjects.getHeight() && map[i + 1, j] != 1)
        {
            itemp = i + 1;
            while (itemp < SetObjects.getHeight() && map[itemp, j] != 1)
                itemp++;
            if (itemp - i < WallGapLengthMax)
            {
                jumlahGap++;
                fitnessTotal += Mathf.Log10((itemp - i) * 10 / WallGapLength);
            }
        }

    }

    public override float getFitnessScore()
    {
        if (jumlahGap > 0)
            return fitnessTotal * weight / jumlahGap;
        return 0;
    }

    public override void resetVariables()
    {
        fitnessTotal = 0;
        jumlahGap = 0;
    }
}
