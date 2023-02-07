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
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain;
using TMPro;

public class GeneticAlgorithmGenerator : MonoBehaviour
{
    [SerializeField] int crossoverMiddleValue;
    [SerializeField] int PanjangWallWeight;
    [SerializeField] int PanjangWallVertikalAmt;
    [SerializeField] int PanjangWallHorizontalAmt;
    TextMeshProUGUI tmpro;

    // Start is called before the first frame update
    void Start()
    {
        int iterasi = 0;
        int length = (SetObjects.getHeight()) * (SetObjects.getWidth());
        double[] kosonganDouble = new double[length];
        double[] kosonganDoubleMax = (double[])kosonganDouble.Clone();
        Array.Fill(kosonganDoubleMax, 4);
        if (PanjangWallVertikalAmt == 0)
            PanjangWallVertikalAmt = Mathf.FloorToInt(SetObjects.getHeight() * 3 / 4);
        if (PanjangWallHorizontalAmt == 0)
            PanjangWallHorizontalAmt = Mathf.FloorToInt(SetObjects.getWidth() * 3 / 8);
        tmpro = gameObject.GetComponent<TextMeshProUGUI>();

        //Kromosom
        var chromosome = new FloatingPointChromosome(kosonganDouble, kosonganDoubleMax, Enumerable.Repeat(3, length).ToArray(), Enumerable.Repeat(0, length).ToArray());
        //Populasi
        var population = new Population(50, 100, chromosome);
        //Fitness
        var fitness = new FuncFitness((c) =>
        {
            iterasi++;
            tmpro.text = "Iterasi ke-" + iterasi;
            var fc = c as FloatingPointChromosome;
            var values = fc.ToFloatingPoints();
            Debug.Log("Dari " + string.Join(",", values));
            int[,] map = deflatten(fc.ToFloatingPoints(), SetObjects.getWidth() , SetObjects.getHeight());
            Debug.Log("Jadi "+String.Join(",", map.Cast<int>()));
            float fitness = 0;
            // Panjang Wall
            fitness += getLengthAndPlayersFitness(map);
            // Aksesibilitas Area Wall
            fitness += getAreaFitness(map);
            // Jarak Kelompok Wall?
            // Aksesibilitas Power Up
            // Rasio Power up dan jumlah powerup
            //bool[,][] flags = new bool[map.GetLength(0), map.GetLength(1)][];
            //Debug.Log("Iterasi ke-" + iterasi + " = " + fitness);
            return Random.Range(0,10) * Random.Range(0, 10);
        });
        //Metode milih ortu
        var selection = new RouletteWheelSelection();
        
        //Metode Crossover
        float r = Random.Range(crossoverMiddleValue, length - crossoverMiddleValue);
        var crossover = new OnePointCrossover(Mathf.RoundToInt(r));
        var mutation = new UniformMutation();
        var termination = new FitnessStagnationTermination(10);

        var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
        ga.Termination = termination;
        ga.Start();
        var a = ga.BestChromosome as FloatingPointChromosome;
        SetObjects.setMap(deflatten(a.ToFloatingPoints(), SetObjects.getWidth(), SetObjects.getHeight()));
        MainMenuNavigation.nextScene();
    }

    int[,] deflatten(double[] arrays, int width, int height)
    {
        int[,] result = new int[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                result[i, j] = (int)arrays[i * height + j];
                //Debug.Log(arrays.Length+","+i + "," + j + "," + arrays[i * height + j] + "," + result[i, j] + "," + (i * height + j));
            }
        }
        return result;
    }

    float getLengthAndPlayersFitness(int[,] map)
    {
        float fitnessvalue = 0;
        float playeramount = 0;
        int jtemp, itemp;
        for (int i = 0; i < SetObjects.getHeight(); i++)
            for (int j = 0; j < SetObjects.getWidth(); j++)
                if (map[i, j] == 1)
                {
                    //Cek Horizontal
                    if (j + 1 < SetObjects.getWidth() && map[i, j + 1] == 1 && (j == 0 || map[i, j - 1] != 1))
                    {
                        jtemp = j;
                        while (jtemp < SetObjects.getWidth() && map[i, jtemp] == 1)
                            jtemp++;
                        fitnessvalue += Mathf.Log10((jtemp - j + 1) * 10 / PanjangWallHorizontalAmt);
                    }
                    //Cek Vertikal
                    if (i + 1 < SetObjects.getHeight() && map[i + 1, j] == 1 && (i == 0 || map[i - 1, j] != 1))
                    {
                        itemp = i;
                        while (itemp < SetObjects.getHeight() && map[itemp, j] == 1)
                            itemp++;
                        fitnessvalue += Mathf.Log10((itemp - i + 1) * 10 / PanjangWallVertikalAmt);
                    }
                    //Cek Diagonal
                    //if (i + 1 < SetObjects.getHeight() && i + 1 < SetObjects.getHeight() && map[i + 1, j + 1] == 1 && (i - 1 >= 0 && j - 1 >= 0 && map[i - 1, j - 1] != 1))
                    //{
                    //    itemp = i;
                    //    jtemp = j;
                    //    while (map[itemp, jtemp] == 1 && itemp < SetObjects.getHeight() && jtemp < SetObjects.getWidth())
                    //    {
                    //        itemp++;
                    //        jtemp++;
                    //    }
                    //    fitnessvalue += Mathf.Log10((itemp - i + 1) * 10 / PanjangWallVertikalAmt);
                    //}
                }
                else if (map[i, j] == 3)
                    playeramount++;
        if (playeramount != 5)
            return fitnessvalue * PanjangWallWeight / 2;
        else
            return fitnessvalue * PanjangWallWeight;
    }

    float getAreaFitness(int[,] map)
    {
        return 0;
    }
}
