using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaFitness : InLoopFitnessBase
{
    ArrayList areaSize;
    int totalSize;
    bool[,] ischecked;

    public override void calculateFitness(int[,] map, Coordinate currCoor)
    {
        int size = 1, i = currCoor.yCoor, j = currCoor.xCoor;
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
            totalSize += (int)areaSize[i];
            if ((int)areaSize[i] > biggest)
                biggest = (int)areaSize[i];
        }

        fitnessTotal = biggest * 2;
        for (int i = 0; i < areaSize.Count; i++)
        {
            fitnessTotal -= (int)areaSize[i];
        }
        return fitnessTotal * weight / totalSize;
    }

    public override void resetVariables()
    {
        areaSize = new ArrayList();
        totalSize = 0;
        ischecked = new bool[SetObjects.getHeight(), SetObjects.getWidth()];
        fitnessTotal = 0;
    }
}
