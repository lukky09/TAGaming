using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGapFitness : InLoopFitnessBase
{
    [Range(0, 10)]
    [SerializeField] int minWallGap;
    [Range(0, 10)]
    [SerializeField] int maxWallGap;
    int jumlahGap;

    // Kalau ini dipakai area besar akan lebih sedikit, mungkin jangan dipakai
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
            if (jtemp - j < maxWallGap)
            {
                jumlahGap++;
                fitnessTotal += Mathf.Log10((jtemp - j) * 10 / minWallGap);
            }
        }
        //Cek Vertikal
        if (i + 1 < SetObjects.getHeight() && map[i + 1, j] != 1)
        {
            itemp = i + 1;
            while (itemp < SetObjects.getHeight() && map[itemp, j] != 1)
                itemp++;
            if (itemp - i < minWallGap)
            {
                jumlahGap++;
                fitnessTotal += Mathf.Log10((itemp - i) * 10 / minWallGap);
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
