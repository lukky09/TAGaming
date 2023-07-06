using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PossibleTemplates
{
    //0 = Kosong
    //1 = Dinding
    //2 = Power Up
    public static int[][,] Templates = new int[][,]
    {
         new int[,]
        {{ 0, 0, 0, 0, 0},
        {  0, 0, 0, 0, 0},
        {  0, 0, 0, 0, 0},
        {  0, 0, 0, 0, 0},
        {  0, 0, 0, 0, 0}},
         new int[,]
        {{ 0, 0, 0, 0, 1},
        {  0, 0, 0, 0, 1},
        {  0, 0, 0, 0, 1},
        {  0, 0, 0, 0, 1},
        {  0, 0, 0, 0, 1}},
         new int[,]
        {{ 0, 0, 0, 0, 1},
        {  0, 0, 0, 0, 1},
        {  0, 0, 0, 0, 1},
        {  0, 0, 0, 0, 1},
        {  1, 1, 1, 1, 1}},
         new int[,]
        {{ 1, 0, 0, 0, 1},
        {  1, 0, 0, 0, 1},
        {  1, 0, 0, 0, 1},
        {  1, 0, 0, 0, 1},
        {  1, 1, 1, 1, 1}},
        new int[,]
        {{ 1, 0, 0, 0, 1},
        {  1, 0, 0, 0, 1},
        {  1, 0, 0, 0, 1},
        {  1, 0, 0, 0, 1},
        {  1, 0, 0, 0, 1}},
        new int[,]
        {{ 1, 1, 0, 1, 1},
        {  1, 0, 0, 0, 1},
        {  0, 0, 0, 0, 0},
        {  1, 0, 0, 0, 1},
        {  1, 1, 0, 1, 1}},
        new int[,]
        {{ 1, 1, 0, 0, 0},
        {  1, 0, 0, 0, 0},
        {  0, 0, 0, 0, 0},
        {  0, 0, 0, 0, 1},
        {  0, 0, 0, 1, 1}},
        new int[,]
        {{ 1, 1, 0, 1, 1},
        {  1, 0, 0, 0, 1},
        {  0, 0, 0, 0, 0},
        {  0, 0, 0, 0, 0},
        {  0, 0, 0, 0, 0}},
         new int[,]
        {{ 0, 0, 0, 0, 0},
        {  0, 0, 0, 0, 0},
        {  1, 1, 1, 1, 1},
        {  0, 0, 0, 0, 0},
        {  0, 0, 0, 0, 0}},
          new int[,]
        {{ 0, 0, 1, 0, 0},
        {  0, 0, 1, 0, 0},
        {  1, 1, 1, 1, 1},
        {  0, 0, 1, 0, 0},
        {  0, 0, 1, 0, 0}},
    };

    public static int[,] getTemplate(int id)
    {
        int tempID = id;
        if (tempID < 0)
        {
            tempID = -tempID - 1;
        }
        int rotation = tempID % 4;
        int[,] chosenTemplate = Templates[Mathf.FloorToInt(tempID / 4)];
        int[,] resultTemplate = new int[5, 5];
        if (rotation == 0)
            resultTemplate = (int[,])chosenTemplate.Clone();
        else if (rotation == 1)
            //rotasi ke kanan
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    resultTemplate[j, 4 - i] = chosenTemplate[i, j];
        else if (rotation == 2)
            //rotasi ke 180 derajat
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    resultTemplate[4 - i, 4 - j] = chosenTemplate[i, j];
        else
            //rotasi ke kiri
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    resultTemplate[4 - j, i] = chosenTemplate[i, j];
        // kalau nomor yang diberi negatif beri power
        if (id < 0 && resultTemplate[2, 2] == 0)
            resultTemplate[2, 2] = 2;
        return resultTemplate;
    }
}
