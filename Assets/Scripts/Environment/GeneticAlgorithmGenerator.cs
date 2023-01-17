using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GeneticSharp.Domain.Chromosomes;
using System;
using static UnityEngine.Rendering.DebugUI;
using System.Linq;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Crossovers;
using Random = UnityEngine.Random;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Domain;

public class GeneticAlgorithmGenerator : MonoBehaviour
{
    [SerializeField] int crossoverMiddleValue;

    Slider sliderReference;

    // Start is called before the first frame update
    void Start()
    {
        sliderReference = gameObject.GetComponent<Slider>();
        int length = (SetObjects.getHeight() - 2) * (SetObjects.getWidth() - 2);
        double[] kosonganDouble = new double[length];
        double[] kosonganDoubleMax = (double[])kosonganDouble.Clone();
        Array.Fill(kosonganDoubleMax, 4);

        //Kromosom
        var chromosome = new FloatingPointChromosome(kosonganDouble, kosonganDoubleMax, Enumerable.Repeat(3, length).ToArray(), Enumerable.Repeat(0, length).ToArray());
        //Populasi
        var population = new Population(50, 100, chromosome);
        //Fitness (nti ganti)
        var fitness = new FuncFitness((c) =>
        {
            var fc = c as FloatingPointChromosome;
            var values = fc.ToFloatingPoints();
            //deflatten(fc.ToFloatingPoints(),SetObjects.getWidth(), SetObjects.getHeight())
            var x1 = values[0];
            var y1 = values[1];
            var x2 = values[2];
            var y2 = values[3];
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        });
        //Metode milih ortu
        var selection = new RouletteWheelSelection();
        //Metode Crossover
        float r = Random.Range(crossoverMiddleValue, length - crossoverMiddleValue);
        var crossover = new OnePointCrossover(Mathf.RoundToInt(r));
        var mutation = new UniformMutation();
        var termination = new FitnessStagnationTermination(50);

        var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
        ga.Termination = termination;
        ga.Start();
    }

    int[,] deflatten(double[] arrays, int width, int height)
    {
        int[,] result = new int[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                result[i, j] = (int)arrays[i * height + width];
            }
        }
        return result;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
