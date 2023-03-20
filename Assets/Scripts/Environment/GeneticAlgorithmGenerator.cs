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
using static UnityEditor.Progress;
using System.Drawing;

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
    [SerializeField] bool useMirrorFitness;
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
    [SerializeField] bool maxRockFitness;
    [SerializeField] float maxRockRatio;
    [SerializeField] int maxRockWeight;
    TextMeshProUGUI tmpro;
    int mapWidth;

    // Start is called before the first frame update
    void Start()
    {
        double fitness;
        if (!useMirrorFitness)
            mapWidth = (int)(SetObjects.getWidth() / 2);
        else
            mapWidth = (int)SetObjects.getWidth();
        float[] tempfitness = new float[5];
        int length = SetObjects.getHeight() * mapWidth;
        if (PanjangWallVertikalAmt == 0)
            PanjangWallVertikalAmt = Mathf.FloorToInt(SetObjects.getHeight() * 3 / 4);
        if (PanjangWallHorizontalAmt == 0)
            PanjangWallHorizontalAmt = Mathf.FloorToInt(SetObjects.getWidth() * 3 / 4);


        tmpro = gameObject.GetComponent<TextMeshProUGUI>();

        //Multithreading
        var taskExecutor = new ParallelTaskExecutor();
        taskExecutor.MinThreads = 12;
        taskExecutor.MaxThreads = 12;

        //Kromosom
        var chromosome = new GameChromosome(length, Mathf.FloorToInt(PURatioAmt * length));
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
        SetObjects.setMap(deflatten(a, mapWidth, SetObjects.getHeight()), !useMirrorFitness);
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
                if (useMirrorFitness)
                    result[i, width - 1 - j] = (int)((Gene)arrays[i * width + j]).Value;
            }
        }
        return result;
    }

    double fitnessFunction(int[,] map)
    {
        int[] objAmt = new int[4];
        double[] fitnessScores = new double[6];
        Coordinate tempCoor;

        //Utk jumlah batu
        int rockGroupAmount = 0;

        //Utk Wall
        int[] wSize = new int[2]; //jumlah [horizontal , vertikal]
        float[] wScore = new float[2]; //skor total  

        //Utk Area
        int totalSize = 0;
        ArrayList areasSize = new ArrayList();
        bool[,] ischecked = new bool[SetObjects.getHeight(), mapWidth];

        //Utk gap wall
        int jumlahGap = 0;

        //untuk aksesibilitas PowerUp
        ArrayList lokasiPlayer = new ArrayList();
        ArrayList lokasiPowerUp = new ArrayList();

        for (int i = 0; i < SetObjects.getHeight(); i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                objAmt[map[i, j]]++;
                tempCoor = new Coordinate(j, i);
                // Fitness Wall
                if (map[i, j] == 1)
                {
                    //Cek Fitness Panjang Wall
                    wallLengthFitness(map, tempCoor, ref wSize, ref wScore);
                    //Cek Fitness Panjang Gap antar Wall
                    wallGapFitness(map, tempCoor, ref fitnessScores, ref jumlahGap);
                    //Cek Jumlah batu dalam 1 kumpulan
                    rockGroupAmount++;
                    fitnessScores[5]+= rockAmountFitness(map,tempCoor,ref ischecked);
                }
                else if (map[i, j] != 1 && !ischecked[i, j])
                {
                    areasSize.Add(getAreaSize(ref ischecked, map, tempCoor));
                }
                //Cek Apakah PowerUp bisa diakses
                if (includePUPAccesibilityFitness)
                {
                    if (map[i, j] == 2)
                        lokasiPowerUp.Add(new Coordinate(j, i));
                    if (map[i, j] == 3)
                        lokasiPlayer.Add(new Coordinate(j, i));
                }
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

        fitnessScores[2] = fitnessScores[2] * WallGapWeight / jumlahGap;
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
                //Ambil player terdekat biar Astar tidak terlalu lama
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

        if (maxRockFitness)
            fitnessScores[5] /= rockGroupAmount * maxRockWeight;

        if (useMirrorFitness)
            objAmt[3] = Mathf.RoundToInt(objAmt[3] / 2);

        Debug.Log(String.Join(" - ", fitnessScores));
        return Mathf.Pow((float)fitnessScores.Sum() / Mathf.Pow(MathF.Abs(objAmt[3] - 5) * 5 + 1, 3), 3);
    }

    float rockAmountFitness(int[,] map, Coordinate coor,ref bool[,] ischecked )
    {
        if (maxRockFitness && !ischecked[coor.yCoor,coor.xCoor])
        {
            int size = 1, i = coor.yCoor, j = coor.xCoor;
            ischecked[i, j] = true;
            Queue<Coordinate> q = new Queue<Coordinate>();
            Coordinate c, tempCoor;
            q.Enqueue(new Coordinate(j, i));
            //Ngambil Ukuran area 1 per 1
            while (q.Count > 0)
            {
                c = q.Dequeue();
                for (int k = -1; k < 2; k++)
                {
                    for (int l = -1; l < 2; l++)
                    {
                        tempCoor = new Coordinate(c.xCoor + l, c.yCoor + k);
                        if (tempCoor.xCoor >= 0 && tempCoor.yCoor >= 0 && tempCoor.yCoor < SetObjects.getHeight() && tempCoor.xCoor < mapWidth && map[tempCoor.yCoor, tempCoor.xCoor] == 1 && !ischecked[tempCoor.yCoor, tempCoor.xCoor])
                        {
                            ischecked[tempCoor.yCoor, tempCoor.xCoor] = true;
                            q.Enqueue(tempCoor);
                            size++;
                        }
                    }
                }
            }
            float maxRockAmount = Mathf.RoundToInt(map.GetLength(0) * map.GetLength(1) * maxRockRatio);
            return Mathf.Abs(maxRockAmount - size) / maxRockAmount * 1.0f;
        }
        return 0;
    }

    void wallLengthFitness(int[,] map, Coordinate coor, ref int[] wallSize, ref float[] wallScore)
    {
        int i = coor.yCoor, j = coor.xCoor;
        int jtemp, itemp;

        if (includeWallFitness)
        {
            //Cek Horizontal
            if (j + 1 < mapWidth && map[i, j + 1] == 1 && (j == 0 || map[i, j - 1] != 1))
            {
                wallSize[0]++;
                jtemp = j;
                while (jtemp < mapWidth && map[i, jtemp] == 1)
                    jtemp++;
                wallScore[0] += Mathf.Log10((jtemp - j + 1) * 10 / PanjangWallHorizontalAmt);
            }
            //Cek Vertikal
            if (i + 1 < SetObjects.getHeight() && map[i + 1, j] == 1 && (i == 0 || map[i - 1, j] != 1))
            {
                wallSize[1]++;
                itemp = i;
                while (itemp < SetObjects.getHeight() && map[itemp, j] == 1)
                    itemp++;
                wallScore[1] += Mathf.Log10((itemp - i + 1) * 10 / PanjangWallVertikalAmt);
            }
        }
    }

    void wallGapFitness(int[,] map, Coordinate coor, ref double[] fitnessScores, ref int gapAmount)
    {
        int i = coor.yCoor, j = coor.xCoor;
        int jtemp, itemp;

        if (includeWallGapFitness)
        {
            //Cek Horizontal
            if (j + 1 < mapWidth && map[i, j + 1] != 1)
            {
                jtemp = j + 1;
                while (jtemp < mapWidth && map[i, jtemp] != 1)
                    jtemp++;
                if (jtemp - j < WallGapLengthMax)
                {
                    gapAmount++;
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
                    gapAmount++;
                    fitnessScores[2] += Mathf.Log10((itemp - i) * 10 / WallGapLength);
                }
            }
        }
    }


    int getAreaSize(ref bool[,] ischecked, int[,] map, Coordinate curr)
    {
        int size = 1, i = curr.yCoor, j = curr.xCoor;
        ischecked[i, j] = true;
        Queue<Coordinate> q = new Queue<Coordinate>();
        Coordinate c,tempCoor;
        q.Enqueue(new Coordinate(j, i));
        //Ngambil Ukuran area 1 per 1
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
        return size;
    }

}