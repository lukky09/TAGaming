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
        var mapValues = RandomizationProvider.Current.GetInts(ukuranMap, 0, 3);
        
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
        return new Gene(RandomizationProvider.Current.GetInt(0, 3));
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
    [SerializeField] bool includeWallFitness;
    [SerializeField] int PanjangWallWeight;
    [SerializeField] int PanjangWallVertikalAmt;
    [SerializeField] int PanjangWallHorizontalAmt;
    [SerializeField] bool includeAreaFitness;
    [SerializeField] int AreaWeight;
    [SerializeField] bool includePURatioFitness;
    [SerializeField] float PURatioAmt;
    [SerializeField] int PURatioWeight;
    [SerializeField] int StagnationTerminationAmt;
    TextMeshProUGUI tmpro;
    int mapWidth;

    // Start is called before the first frame update
    void Start()
    {
        double fitness;
        mapWidth = (int)(SetObjects.getWidth() / 2);
        float[] tempfitness = new float[5];
        int length = (SetObjects.getHeight()) * (SetObjects.getWidth()/2);
        Debug.Log(length);
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
            fitness = fitnessFunction(map);
            //Debug.Log(fitness);
            return fitness;
        });
        //Metode milih ortu
        var selection = new RouletteWheelSelection();
        
        //Metode Crossover
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

    double fitnessFunction(int[,] map)
    {
        int[] objAmt = new int[4];
        double[] fitnessScores = new double[5];
        //Utk Wall
        int jtemp, itemp;
        //Utk Area
        int size;
        Queue<Coordinate> q;
        Coordinate c, tempCoor;
        ArrayList areasSize = new ArrayList();
        bool[,] ischecked = new bool[SetObjects.getHeight(), mapWidth];

        for (int i = 0; i < SetObjects.getHeight(); i++)
            for (int j = 0; j < mapWidth; j++)
            {
                objAmt[map[i, j]]++;
                // Fitness Wall
                if (map[i, j] == 1 && includeWallFitness)
                {
                    //Cek Horizontal
                    if (j + 1 < mapWidth && map[i, j + 1] == 1 && (j == 0 || map[i, j - 1] != 1))
                    {
                        jtemp = j;
                        while (jtemp < mapWidth && map[i, jtemp] == 1)
                            jtemp++;
                        fitnessScores[0] += Mathf.Log10((jtemp - j + 1) * 10 / PanjangWallHorizontalAmt);
                    }
                    //Cek Vertikal
                    if (i + 1 < SetObjects.getHeight() && map[i + 1, j] == 1 && (i == 0 || map[i - 1, j] != 1))
                    {
                        itemp = i;
                        while (itemp < SetObjects.getHeight() && map[itemp, j] == 1)
                            itemp++;
                        fitnessScores[0] += Mathf.Log10((itemp - i + 1) * 10 / PanjangWallVertikalAmt);
                    }
                }
                else if (map[i, j] != 1 && !ischecked[i, j])
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
            }
        fitnessScores[0] *= PanjangWallWeight;
        int biggest = -999;

        // Ini aku pakai area yang bisa diakses player, bukan panjang * lebar Arena
        for (int i = 0; i < areasSize.Count; i++)
        {
            if ((int)areasSize[i] > biggest)
                biggest = (int)areasSize[i];
        }
        if (includeAreaFitness)
        {
            fitnessScores[1] = biggest * 2;
            for (int i = 0; i < areasSize.Count; i++)
            {
                fitnessScores[1] -= (int)areasSize[i];
            }
            fitnessScores[1] *= AreaWeight;
        }
        if (includePURatioFitness)
        {
            fitnessScores[4] = -(float)Math.Pow(((objAmt[2] / biggest) - PURatioAmt) * 100, 2) * PURatioWeight;
        }
        //Debug.Log(String.Join(',', fitnessScores));
        return fitnessScores.Sum() / Mathf.Pow(MathF.Abs(objAmt[3] - 5) * 5 + 1, 3);
    }
}
