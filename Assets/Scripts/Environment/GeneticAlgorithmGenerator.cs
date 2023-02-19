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
using System.Threading.Tasks;
using GeneticSharp.Infrastructure.Framework.Threading;

public class GameChromosome : ChromosomeBase
{
    private readonly int m_ukuranMap;
    private readonly int m_powerup;
    public GameChromosome(int ukuranMap,int powerup) : base(ukuranMap)
    {
        int temp;
        m_ukuranMap = ukuranMap;
        m_powerup = powerup;
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
        for (int i = 0; i < powerup; i++)
        {
            temp = Mathf.FloorToInt(Random.Range(0, ukuranMap));
            while (mapValues[temp] == 3 || mapValues[temp] == 2)
                temp = Mathf.FloorToInt(Random.Range(0, ukuranMap));
            ReplaceGene(temp, new Gene(2));
        }
    }

    public override Gene GenerateGene(int geneIndex)
    {
        return new Gene(RandomizationProvider.Current.GetInt(0, 3));
    }

    public override IChromosome CreateNew()
    {
        return new GameChromosome(m_ukuranMap,m_powerup);
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
    [SerializeField] bool includeWallGapFitness;
    [SerializeField] int WallGapLength;
    [SerializeField] int WallGapLengthMax;
    [SerializeField] int WallGapWeight;
    [SerializeField] bool includePUPAccesibilityFitness;
    [SerializeField] float PUPAccesibilityWeight;
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
        if (PanjangWallVertikalAmt == 0)
            PanjangWallVertikalAmt = Mathf.FloorToInt(SetObjects.getHeight() * 3 / 4);
        if (PanjangWallHorizontalAmt == 0)
            PanjangWallHorizontalAmt = Mathf.FloorToInt(SetObjects.getWidth() * 3 / 2);
        tmpro = gameObject.GetComponent<TextMeshProUGUI>();

        //Multithreading
        var taskExecutor = new ParallelTaskExecutor();
        taskExecutor.MinThreads = 12;
        taskExecutor.MaxThreads = 12;

        //Kromosom
        var chromosome = new GameChromosome(length,Mathf.FloorToInt(PURatioAmt * length));
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
        //ga.TaskExecutor = taskExecutor;

        ga.GenerationRan += (sender, e) =>
        {
            tmpro.text = "Iterasi ke-" + ga.GenerationsNumber;
        };

        ga.Start();

        var a = ga.BestChromosome.GetGenes();
        SetObjects.setMap(deflatten(a, mapWidth, SetObjects.getHeight()),true);
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
        int[] wSize = new int[2]; //jumlah [horizontal - vertikal]
        float[] wScore = new float[2]; //skor total  

        //Utk Area
        int size, totalSize = 0;
        Queue<Coordinate> q;
        Coordinate c, tempCoor;
        ArrayList areasSize = new ArrayList();
        bool[,] ischecked = new bool[SetObjects.getHeight(), mapWidth];

        //Utk gap wall
        int jumlahGap = 0;

        //untuk aksesibilitas PowerUp
        ArrayList lokasiPlayer = new ArrayList();
        ArrayList lokasiPowerUp = new ArrayList();

        for (int i = 0; i < SetObjects.getHeight(); i++)
            for (int j = 0; j < mapWidth; j++)
            {
                objAmt[map[i, j]]++;
                // Fitness Wall
                if (map[i, j] == 1)
                {
                    if (includeWallFitness)
                    {
                        //Cek Horizontal
                        if (j + 1 < mapWidth && map[i, j + 1] == 1 && (j == 0 || map[i, j - 1] != 1))
                        {
                            wSize[0]++;
                            jtemp = j;
                            while (jtemp < mapWidth && map[i, jtemp] == 1)
                                jtemp++;
                            wScore[0] += Mathf.Log10((jtemp - j + 1) * 10 / PanjangWallHorizontalAmt);
                        }
                        //Cek Vertikal
                        if (i + 1 < SetObjects.getHeight() && map[i + 1, j] == 1 && (i == 0 || map[i - 1, j] != 1))
                        {
                            wSize[1]++;
                            itemp = i;
                            while (itemp < SetObjects.getHeight() && map[itemp, j] == 1)
                                itemp++;
                            wScore[1] += Mathf.Log10((itemp - i + 1) * 10 / PanjangWallVertikalAmt);
                        }
                    }
                    if (includeWallGapFitness)
                    {
                        //Cek Horizontal
                        if (j + 1 < mapWidth && map[i, j + 1] != 1)
                        {
                            jtemp = j + 1;
                            while (jtemp < mapWidth && map[i, jtemp] != 1 )
                                jtemp++;
                            if (jtemp - j < WallGapLengthMax)
                            {
                                jumlahGap++;
                                fitnessScores[2] += Mathf.Log10((jtemp - j) * 10 / WallGapLength);
                            }
                        }
                        //Cek Vertikal
                        if (i + 1 < SetObjects.getHeight() && map[i + 1, j] != 1)
                        {
                            itemp = i + 1;
                            while (itemp < SetObjects.getHeight() && map[itemp, j] != 1)
                                itemp++;
                            if (itemp - i < WallGapLengthMax)
                            {
                                jumlahGap++;
                                fitnessScores[2] += Mathf.Log10((itemp - i) * 10 / WallGapLength);
                            }
                        }
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
                if (includePUPAccesibilityFitness)
                {
                    if (map[i, j] == 2)
                        lokasiPowerUp.Add(new Coordinate(j, i));
                    if (map[i, j] == 3)
                        lokasiPlayer.Add(new Coordinate(j, i));
                }
            }

        if (includeWallFitness)
        {
            if (wSize[0] > 0)
                fitnessScores[0] += wScore[0] / wSize[0];
            if (wSize[1] > 0)
                fitnessScores[0] += wScore[1] / wSize[1];
            fitnessScores[0] = (fitnessScores[0] / 2) * PanjangWallWeight;
        }

        fitnessScores[2]  = fitnessScores[2] * WallGapWeight / jumlahGap;
        float biggest = -999;

        // Ini aku pakai area yang bisa diakses player, bukan panjang * lebar Arena
        for (int i = 0; i < areasSize.Count; i++)
        {
            totalSize += (int)areasSize[i];
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
            fitnessScores[1] = fitnessScores[1] * AreaWeight / totalSize;
        }

        if (includePURatioFitness)
        {
            fitnessScores[4] = (1 - MathF.Abs((objAmt[2] / biggest) - PURatioAmt)) * PURatioWeight;
        }

        //Setiap powerup akan dicek bila bisa dicapai player terdekat
        int indexPlayer = 0;
        float tempDistance;
        if (includePUPAccesibilityFitness && lokasiPlayer.Count > 0)
        {
            for (int i = 0; i < lokasiPowerUp.Count; i++)
            {
                biggest = 999;
                for (int j = 0; j < lokasiPlayer.Count; j++)
                {
                    tempDistance = Coordinate.Distance((Coordinate)lokasiPowerUp[i], (Coordinate)lokasiPlayer[j]);
                    if (tempDistance < biggest)
                    {
                        biggest = tempDistance;
                        indexPlayer = j;
                    }
                }
                if (AStarAlgorithm.doAstarAlgo((Coordinate)lokasiPowerUp[i], (Coordinate)lokasiPlayer[indexPlayer], map) != null)
                {
                    fitnessScores[3]++;
                }
            }
            if (lokasiPowerUp.Count == 0)
                fitnessScores[3] = 0;
            else
                fitnessScores[3] = (fitnessScores[3] / lokasiPowerUp.Count) * PUPAccesibilityWeight;
        }


        Debug.Log(String.Join(" - ", fitnessScores));
        return fitnessScores.Sum() / Mathf.Pow(MathF.Abs(objAmt[3] - 5) * 5 + 1, 3);
    }
}
