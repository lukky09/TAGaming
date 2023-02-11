using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticSharp.Domain.Chromosomes;
using System;
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

public class GameChromosome : ChromosomeBase
{
    private readonly int m_ukuranMap;
    public GameChromosome(int ukuranMap) : base(ukuranMap)
    {
        int temp;
        m_ukuranMap = ukuranMap;
        var mapValues = RandomizationProvider.Current.GetInts(ukuranMap, 0, 2);
        for (int i = 0; i < ukuranMap; i++)
        {
            ReplaceGene(i, new Gene(mapValues[i]));
        }
        for (int i = 0; i < 5; i++)
        {
            temp = Mathf.FloorToInt(Random.Range(0, ukuranMap));
            while (mapValues[temp] == 3)
                temp = Mathf.FloorToInt(Random.Range(0, ukuranMap));
            ReplaceGene(temp, new Gene(3));
        }
    }

    public override Gene GenerateGene(int geneIndex)
    {
        return new Gene(RandomizationProvider.Current.GetInt(0, m_ukuranMap));
    }

    public override IChromosome CreateNew()
    {
        return new GameChromosome(m_ukuranMap);
    }

    public override IChromosome Clone()
    {
        var clone = base.Clone() as GameChromosome;
        return clone;
    }
}

public class GeneticAlgorithmGenerator : MonoBehaviour
{
    [SerializeField] int crossoverMiddleValue;
    [SerializeField] int PanjangWallWeight;
    [SerializeField] int PanjangWallVertikalAmt;
    [SerializeField] int PanjangWallHorizontalAmt;
    [SerializeField] int AreaWeight;
    [SerializeField] int StagnationTerminationAmt;
    TextMeshProUGUI tmpro;
    int mapWidth;

    // Start is called before the first frame update
    void Start()
    {
        mapWidth = (int)(SetObjects.getWidth() / 2);
        int jumPlayer;
        float temp1, temp2, fitness;
        int length = (SetObjects.getHeight()) * (SetObjects.getWidth()/2);
        if (PanjangWallVertikalAmt == 0)
            PanjangWallVertikalAmt = Mathf.FloorToInt(SetObjects.getHeight() * 3 / 4);
        if (PanjangWallHorizontalAmt == 0)
            PanjangWallHorizontalAmt = Mathf.FloorToInt(SetObjects.getWidth() * 3 / 2);
        tmpro = gameObject.GetComponent<TextMeshProUGUI>();

        //Kromosom
        var chromosome = new GameChromosome(length);
        //Populasi
        var population = new Population(50, 100, chromosome);
        //Fitness
        var fitnessfunc = new FuncFitness((c) =>
        {
            var fc = c.GetGenes();
            int[,] map = deflatten(fc, mapWidth, SetObjects.getHeight());
            fitness = 0;
            // Panjang Wall
            (jumPlayer, temp1) = getLengthAndPlayersFitness(map);
            fitness += temp1;
            // Aksesibilitas Area Wall
            temp2 = getAreaFitness(map);
            fitness += temp2;
            // Jarak Kelompok Wall?
            // Aksesibilitas Power Up
            // Rasio Power up dan jumlah powerup
            fitness -= Mathf.Pow(jumPlayer - 5, 3);
            Debug.Log(temp1 + "," + temp2 + "," + jumPlayer+" = "+fitness);
            return fitness;
        });
        //Metode milih ortu
        var selection = new RouletteWheelSelection();
        
        //Metode Crossover
        float r = Random.Range(crossoverMiddleValue, length - crossoverMiddleValue);
        var crossover = new UniformCrossover();
        var mutation = new UniformMutation(false);
        var termination = new FitnessStagnationTermination(StagnationTerminationAmt);

        var ga = new GeneticAlgorithm(population, fitnessfunc, selection, crossover, mutation);
        ga.Termination = termination;

        ga.GenerationRan += (sender, e) =>
        {
            tmpro.text = "Iterasi ke-" + ga.GenerationsNumber;
        };

        ga.Start();

        var a = ga.BestChromosome.GetGenes();
        SetObjects.setMap(deflatten(a, mapWidth, SetObjects.getHeight()));
        MainMenuNavigation.nextScene();
    }

    int[,] deflatten(Gene[] arrays, int width, int height)
    {
        int[,] result = new int[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                result[i, j] = (int)((Gene)arrays[i * width + j]).Value;
            }
        }
        return result;
    }

    (int,float) getLengthAndPlayersFitness(int[,] map)
    {
        float fitnessvalue = 0;
        int playeramount = 0;
        int jtemp, itemp;
        for (int i = 0; i < SetObjects.getHeight(); i++)
            for (int j = 0; j < mapWidth; j++)
                if (map[i, j] == 1)
                {
                    //Cek Horizontal
                    if (j + 1 < mapWidth && map[i, j + 1] == 1 && (j == 0 || map[i, j - 1] != 1))
                    {
                        jtemp = j;
                        while (jtemp < mapWidth && map[i, jtemp] == 1)
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
        return (playeramount,fitnessvalue * PanjangWallWeight);
    }

    float getAreaFitness(int[,] map)
    {
        Queue<Coordinate> q;
        Coordinate c, tempCoor;
        ArrayList areasSize = new ArrayList();
        int size;
        bool[,] ischecked = new bool[SetObjects.getHeight(), mapWidth];
        for (int i = 0; i < SetObjects.getHeight(); i++)
            for (int j = 0; j < mapWidth; j++)
                if (map[i, j] != 1 && !ischecked[i, j])
                {
                    size = 1;
                    ischecked[i, j] = true;
                    q = new Queue<Coordinate>();
                    q.Enqueue(new Coordinate(j, i));
                    while (q.Count > 0)
                    {
                        c = q.Dequeue();

                        for (int k = 0; k < 4; k++)
                        {
                            tempCoor = new Coordinate(c.xCoor + Mathf.RoundToInt(Mathf.Sin(k * Mathf.PI / 2)), c.yCoor + Mathf.RoundToInt(Mathf.Cos(k * Mathf.PI / 2)));
                            if (tempCoor.xCoor >= 0 && tempCoor.yCoor >= 0 && tempCoor.yCoor < SetObjects.getHeight() && tempCoor.xCoor < mapWidth && map[tempCoor.yCoor, tempCoor.xCoor] != 1 && !ischecked[tempCoor.yCoor, tempCoor.xCoor])
                            {
                                ischecked[tempCoor.yCoor, tempCoor.xCoor] = true;
                                q.Enqueue(tempCoor);
                                size++;
                            }

                        }
                    }
                    areasSize.Add(size);
                }
        int biggest = -999;
        float fitness = 0;

        // Ini aku pakai area yang bisa diakses player, bukan panjang * lebar Arena
        if (areasSize.Count > 1)
        {
            for (int i = 0; i < areasSize.Count; i++)
            {
                fitness += (int)areasSize[i];
                if ((int)areasSize[i] > biggest)
                    biggest = (int)areasSize[i];
            }
            fitness = biggest;
        }
        else
        {
            fitness = (int)areasSize[0];
        }
        return fitness * AreaWeight;
    }
}
