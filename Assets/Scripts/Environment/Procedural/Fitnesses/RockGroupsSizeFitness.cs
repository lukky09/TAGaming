using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Search;
using UnityEngine;

public class RockGroupsSizeFitness : InLoopFitnessBase
{
    [SerializeField] bool inRatioFormat;
    [Range(0.0f, 100.0f)]
    [SerializeField] float minRockAmount;
    [Range(0.0f, 100.0f)]
    [SerializeField] float maxRockAmount;
    bool[,] ischecked;
    int rockGroupAmount;

    //Fitness ini mengambil ukuran dari sebuah area dan dibandingkan dengan rasio ukuran yang diminta
    public override void calculateFitness(int[,] map, Coordinate currCoor)
    {
        if (map[currCoor.yCoor, currCoor.xCoor] != 1 || ischecked[currCoor.yCoor, currCoor.xCoor])
            return;

        rockGroupAmount++;
        int size = 1, i = currCoor.yCoor, j = currCoor.xCoor;
        ischecked[i, j] = true;
        Queue<Coordinate> q = new();
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
                    if (tempcoor.xCoor >= 0 && tempcoor.yCoor >= 0 && tempcoor.yCoor < SetObjects.getHeight() && tempcoor.xCoor < SetObjects.getWidth() && map[tempcoor.yCoor, tempcoor.xCoor] == 1 && !ischecked[tempcoor.yCoor, tempcoor.xCoor])
                    {
                        ischecked[tempcoor.yCoor, tempcoor.xCoor] = true;
                        q.Enqueue(tempcoor);
                        size++;
                    }
                }
            }
        }

        int maxRock, minrock;


        if (inRatioFormat)
        {
            maxRock = Mathf.RoundToInt(map.GetLength(0) * map.GetLength(1) * maxRockAmount / 100);
            minrock = Mathf.RoundToInt(map.GetLength(0) * map.GetLength(1) * minRockAmount / 100);
        }
        else
        {
            maxRock = Mathf.FloorToInt(maxRockAmount);
            minrock = Mathf.FloorToInt(minRockAmount);
        }
        if (maxRock < minrock)
        {
            Debug.LogWarning("Min dan max ditukar");
            (minrock, maxRock) = (maxRock, minrock);
        }

        float nilaiMinus = 0;
        if (size < minrock)
            nilaiMinus = minrock - size;
        else if (size > maxRock)
            nilaiMinus = size - maxRock;

        float nilaiMinusMax = SetObjects.getWidth() * SetObjects.getHeight() - maxRock > minrock ? SetObjects.getWidth() * SetObjects.getHeight() - maxRock : minrock;

        nilaiMinus /= nilaiMinusMax;
        //Debug.Log($"1 - (Size : {size}){nilaiMinus} / {(maxRock - minrock)} = {1 - (nilaiMinus / (maxRock - minrock))}");
        //Debug.Log($"Dapet {size} dibandingkan dengan {maxRock} Dapet {nilaiMinus} ({Mathf.Pow(1 - nilaiMinus / (maxRock - minrock), 2)}) ");
        //Fitness Total akan ditambah dengan 1 - beda antara ekspektasi dan jumlah batu per kelompok
        fitnessTotal += 1 - nilaiMinus;
    }

    override public void resetVariables()
    {
        ischecked = new bool[SetObjects.getHeight(), SetObjects.getWidth()];
        rockGroupAmount = 0;
        fitnessTotal = 0;
    }

    override public float getFitnessScore()
    {
        if (fitnessTotal > 0)
        {
            //Kembalikan rata - rata
            return Mathf.Pow(fitnessTotal / rockGroupAmount, 2) * weight;
        }
        else
        {
            return 0;
        }
    }

}
