using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TemplateVarietyFitness : InLoopFitnessBase
{
    [SerializeField] int variedTemplateTolerance;
    int[,] savedMap;

    public override void calculateFitness(int[,] map, Coordinate currCoor)
    {
        return;
    }

    //Khusus Template Variety karena dia pake template map bukan map biasanya
    public void getTemplateMap(int[,] map)
    {
        savedMap = map;
    }

    public override float getFitnessScore()
    {
        SortedList<int, int> templateOccurences = new SortedList<int, int>();
        for (int i = 0; i < savedMap.GetLength(0); i++)
        {
            for (int j = 0; j < savedMap.GetLength(1); j++)
            {
                if (templateOccurences.ContainsKey(savedMap[i, j]))
                    templateOccurences.Add(savedMap[i, j], 1);
                else
                    templateOccurences[savedMap[i, j]]++;
            }
        }
        int[] scores = new int[templateOccurences.Count];
        int temp;
        for (int i = 0; i < templateOccurences.Count; i++)
        {
            temp = templateOccurences.Values[i]-variedTemplateTolerance < 0 ? -(templateOccurences.Values[i] - variedTemplateTolerance) : 0;
            scores[i] = 1 - temp;
        }

        return (scores.Sum() / scores.Length) * weight;
    }

    public override void resetVariables()
    {
        return;
    }
}
