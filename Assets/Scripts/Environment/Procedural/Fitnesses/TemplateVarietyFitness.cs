using GeneticSharp.Domain.Chromosomes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TemplateVarietyFitness : InLoopFitnessBase
{
    [SerializeField] int variedTemplateTolerance;
    int[] savedMap;

    public override void calculateFitness(int[,] map, Coordinate currCoor)
    {
        return;
    }

    //Khusus Template Variety karena dia pake template map bukan map biasanya
    public void getTemplateMap(Gene[] map)
    {
        
        savedMap = new int[map.Length];
        for (int i = 0; i < map.Length; i++)
        {
            savedMap[i] = (int)map[i].Value;
        }
    }

    public override float getFitnessScore()
    {
        SortedList<int, int> templateOccurences = new();
        int temp;
        for (int i = 0; i < savedMap.Length; i++)
        {
            temp = savedMap[i] >= 0 ? savedMap[i] : -savedMap[i] - 1;
            if (templateOccurences.ContainsKey(temp))
                templateOccurences[temp]++;
            else
                templateOccurences.Add(temp, 1);
        }
        int[] scores = new int[templateOccurences.Count];
        for (int i = 0; i < templateOccurences.Count; i++)
        {
            temp = variedTemplateTolerance  - templateOccurences.Values[i] < 0 ? variedTemplateTolerance - templateOccurences.Values[i] : 0;
            scores[i] = 1 + temp;
        }

        float sum = scores.Sum();
        if (sum < 0)
            sum = 0;
        return Mathf.Pow(sum / scores.Length,2) * weight;
    }

    public override void resetVariables()
    {
        return;
    }
}
