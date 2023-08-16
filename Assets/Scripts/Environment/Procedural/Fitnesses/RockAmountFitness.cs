using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Search;
using UnityEngine;

public class RockAmountFitness : InLoopFitnessBase
{
    [Range(0.0f, 100.0f)]
    [SerializeField] float maxRockRatio;
    bool[,] ischecked;
    int rockGroupAmount;

    public override void calculateFitness(int[,] map, Coordinate currCoor)
    {
        int mapWidth = SetObjects.getWidth() / 2;
        if (ischecked == null)
            ischecked = new bool[SetObjects.getHeight(), mapWidth];

        if (map[currCoor.yCoor, currCoor.xCoor] != 1 || ischecked[currCoor.yCoor, currCoor.xCoor])
            return;

        rockGroupAmount++;
        int size = 1, i = currCoor.yCoor, j = currCoor.xCoor;
        ischecked[i, j] = true;
        Queue<Coordinate> q = new Queue<Coordinate>();
        Coordinate c, tempcoor;
        q.Enqueue(new Coordinate(j, i));
        //ngambil ukuran area 1 per 1
        while (q.Count > 0)
        {
            c = q.Dequeue();
            for (int k = -1; k < 2; k++)
            {
                for (int l = -1; l < 2; l++)
                {
                    //Cek 8 Arah
                    tempcoor = new Coordinate(c.xCoor + l, c.yCoor + k);
                    if (tempcoor.xCoor >= 0 && tempcoor.yCoor >= 0 && tempcoor.yCoor < SetObjects.getHeight() && tempcoor.xCoor < mapWidth && map[tempcoor.yCoor, tempcoor.xCoor] == 1 && !ischecked[tempcoor.yCoor, tempcoor.xCoor])
                    {
                        ischecked[tempcoor.yCoor, tempcoor.xCoor] = true;
                        q.Enqueue(tempcoor);
                        size++;
                    }
                }
            }
        }
        float maxRockAmount = Mathf.RoundToInt(map.GetLength(0) * map.GetLength(1) * maxRockRatio / 100);
        //Fitness Total akan ditambah dengan 1 - beda antara ekspektasi dan jumlah batu
        fitnessTotal += 1 - Mathf.Log10(((Mathf.Abs(maxRockAmount - size) + 1) * 10) / (maxRockAmount + 1));
    }

    override public void resetVariables()
    {
        ischecked = new bool[SetObjects.getHeight(), SetObjects.getWidth()];
        rockGroupAmount = 0;
        fitnessTotal = 0;
    }

    override public float getFitnessScore()
    {
        return (fitnessTotal / rockGroupAmount) * weight;
    }

}
