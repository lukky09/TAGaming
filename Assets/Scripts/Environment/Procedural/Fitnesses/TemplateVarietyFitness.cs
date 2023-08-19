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
        for (int i = 0; i < savedMap.Length; i++)
        {
            if (templateOccurences.ContainsKey(savedMap[i]))
                templateOccurences[savedMap[i]]++;
            else
                templateOccurences.Add(savedMap[i], 1);
        }
        int[] scores = new int[templateOccurences.Count];
        int temp;
        for (int i = 0; i < templateOccurences.Count; i++)
        {
            temp = templateOccurences.Values[i] - variedTemplateTolerance < 0 ? -(templateOccurences.Values[i] - variedTemplateTolerance) : 0;
            scores[i] = 1 + temp;
        }

        return Mathf.Pow(scores.Sum() / scores.Length,2) * weight;
    }

    public override void resetVariables()
    {
        return;
    }
}
