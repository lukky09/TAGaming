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
    [SerializeField] int PanjangWallWeight;
    [SerializeField] int PanjangWallVertikalAmt;
    [SerializeField] int PanjangWallHorizontalAmt;
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
            int[,] map = deflatten(fc.ToFloatingPoints(), SetObjects.getWidth(), SetObjects.getHeight());
            float fitness = 0;
            // Panjang Wall
            fitness += getLengthFitness(map);
            // Aksesibilitas Area Wall
            // Jarak Kelompok Wall?
            // Aksesibilitas Power Up
            // Rasio Power up dan jumlah powerup
            bool[,][] flags = new bool[map.GetLength(0), map.GetLength(1)][];
            return 1;
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
    float getLengthFitness(int[,] map)
    {
        float fitnessvalue = 0;
        int jtemp, itemp;
        for (int i = 0; i < SetObjects.getHeight(); i++)
            for (int j = 0; j < SetObjects.getWidth(); j++)
                if (map[i, j] == 1)
                {
                    //Cek Horizontal
                    if (j + 1 < SetObjects.getWidth() && map[i, j + 1] == 1 && (j == 0 || map[i, j - 1] != 1))
                    {
                        jtemp = j;
                        while (map[i, jtemp] == 1 && jtemp < SetObjects.getWidth())
                            jtemp++;
                        fitnessvalue += Mathf.Log10((jtemp - j + 1) * 10 / PanjangWallHorizontalAmt);
                    }
                    //Cek Vertikal
                    if (i + 1 < SetObjects.getHeight() && map[i+1, j] == 1 && (i == 0 || map[i-1,j] != 1))
                    {
                        itemp = i;
                        while (map[itemp, j] == 1 && itemp < SetObjects.getHeight())
                            itemp++;
                        fitnessvalue += Mathf.Log10((itemp - i + 1) * 10 / PanjangWallVertikalAmt);
                    }
                    //Cek Diagonal
                    if (i + 1 < SetObjects.getHeight() && i + 1 < SetObjects.getHeight() && map[i + 1, j+1] == 1 && (i-1 >= 0 && j-1 >= 0 && map[i - 1, j-1] != 1))
                    {
                        itemp = i;
                        jtemp = j;
                        while (map[itemp, jtemp] == 1 && itemp < SetObjects.getHeight() && jtemp< SetObjects.getWidth())
                        {
                            itemp++;
                            jtemp++;
                        }
                        fitnessvalue += Mathf.Log10((itemp - i + 1) * 10 / PanjangWallVertikalAmt);
                    }
                }
        return fitnessvalue * PanjangWallWeight;
    }
}
