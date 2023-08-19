using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class WallLengthFitness : InLoopFitnessBase
{
    [SerializeField] int PanjangWallVertikalAmt;
    [SerializeField] int PanjangWallHorizontalAmt;
    int[] wSize = new int[2]; //jumlah [horizontal , vertikal]
    float[] wScore = new float[2]; //skor total  

    //Jujur ini gak perlu dipakai seh
    public override void calculateFitness(int[,] map, Coordinate currCoor)
    {
        int i = currCoor.yCoor, j = currCoor.xCoor;
        int jtemp, itemp;

        if (map[i, j] != 1)
            return;

        int mapWidth = SetObjects.getWidth();
        //Cek Horizontal
        if (j + 1 < mapWidth && map[i, j + 1] == 1 && (j == 0 || map[i, j - 1] != 1))
        {
            wSize[0]++;
            jtemp = j;
            while (jtemp < mapWidth && map[i, jtemp] == 1)
                jtemp++;
            wScore[0] += Mathf.Log10((jtemp - j + 1) * 10 / PanjangWallHorizontalAmt);
        }
        //Cek Vertikal
        if (i + 1 < SetObjects.getHeight() && map[i + 1, j] == 1 && (i == 0 || map[i - 1, j] != 1))
        {
            wSize[1]++;
            itemp = i;
            while (itemp < SetObjects.getHeight() && map[itemp, j] == 1)
                itemp++;
            wScore[1] += Mathf.Log10((itemp - i + 1) * 10 / PanjangWallVertikalAmt);
        }
    }

    public override float getFitnessScore()
    {
        if (wSize[0] > 0)
            fitnessTotal += wScore[0] / wSize[0];
        if (wSize[1] > 0)
            fitnessTotal += wScore[1] / wSize[1];
        return (fitnessTotal / 2) * weight;
    }

    public override void resetVariables()
    {
        fitnessTotal = 0;
        wSize = new int[2];
        wScore = new float[2];
        if (PanjangWallVertikalAmt == 0)
            PanjangWallVertikalAmt = Mathf.FloorToInt(SetObjects.getHeight() * 3 / 4);
        if (PanjangWallHorizontalAmt == 0)
            PanjangWallHorizontalAmt = Mathf.FloorToInt(SetObjects.getWidth() * 3 / 4);
    }
}
