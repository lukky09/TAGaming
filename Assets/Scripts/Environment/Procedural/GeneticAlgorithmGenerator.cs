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
using Newtonsoft.Json.Linq;

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

public class TemplatedMapChromosome : ChromosomeBase
{
    private readonly int m_ukuranMap;
    public TemplatedMapChromosome(int ukuranMap) : base(ukuranMap)
    {
        int temp;
        m_ukuranMap = ukuranMap;
        var mapValues = RandomizationProvider.Current.GetInts(ukuranMap, -(PossibleTemplates.getTemplateAmount()), (PossibleTemplates.getTemplateAmount()) - 1);

        for (int i = 0; i < ukuranMap; i++)
        {
            ReplaceGene(i, new Gene(mapValues[i]));
        }
    }

    public override Gene GenerateGene(int geneIndex)
    {
        return new Gene(RandomizationProvider.Current.GetInt(-(PossibleTemplates.getTemplateAmount()) , (PossibleTemplates.getTemplateAmount())-1));
    }

    public override IChromosome CreateNew()
    {
        return new TemplatedMapChromosome(m_ukuranMap);
    }

    public override IChromosome Clone()
    {
        var clone = base.Clone() as TemplatedMapChromosome;
        return clone;
    }
}

public class GeneticAlgorithmGenerator : MonoBehaviour
{
    bool useMirrorFitness = true;
    [SerializeField] bool useTemplatedGeneration;
    [SerializeField] int StagnationTerminationAmt;
    InLoopFitnessBase[] fitnesses;
    int mapWidth;

    // Start is called before the first frame update
    void Start()
    {
        fitnesses = GetComponents<InLoopFitnessBase>();
        for (int i = 0; i < fitnesses.Length; i++)
        {
            //Ngilangin Fitness yang ngga dipakai
            if (!fitnesses[i].IsUsed || !(useTemplatedGeneration && fitnesses[i].ForTemplateGen) || !(!useTemplatedGeneration && fitnesses[i].ForTileGen))
                fitnesses[i] = null;
        }
        fitnesses = fitnesses.Where(f => f != null).ToArray();

        double fitness;
        useTemplatedGeneration = MainMenuNavigation.isTemplate;
        int length = SetObjects.getHeight() * SetObjects.getWidth();

        //Multithreading
        var taskExecutor = new ParallelTaskExecutor();
        taskExecutor.MinThreads = 12;
        taskExecutor.MaxThreads = 12;

        ChromosomeBase chromosome;
        //Kromosom
        if (useTemplatedGeneration)
         chromosome = new TemplatedMapChromosome(Mathf.RoundToInt(length / 25));
        else
        {
            Debug.Log(length);
            chromosome = new GameChromosome(length, Mathf.FloorToInt(GetComponent<PowerUpRatioFitness>().getRatio() * length));
        }

        //Populasi
        var population = new Population(50, 100, chromosome);
        //Fitness
        var fitnessfunc = new FuncFitness((c) =>
        {
            var fc = c.GetGenes();
            int[,] map;
            if (useTemplatedGeneration)
                map = putPlayerinTemplate(fc, mapWidth, SetObjects.getHeight());
            else
                map = deflatten(fc, mapWidth, SetObjects.getHeight());
            fitness = fitnessFunction(map);
            //Debug.Log(fitness);
            return fitness;
        });
        //Metode milih ortu
        var selection = new RouletteWheelSelection();

        //Metode Crossover
        var crossover = new UniformCrossover();
        var mutation = new PartialShuffleMutation();
        var termination = new FitnessStagnationTermination(StagnationTerminationAmt);

        var ga = new GeneticAlgorithm(population, fitnessfunc, selection, crossover, mutation);
        ga.Termination = termination;
        //ga.TaskExecutor = taskExecutor;

        ga.Start();

        var a = ga.BestChromosome.GetGenes();
        if (useTemplatedGeneration)
            SetObjects.setMap(putPlayerinTemplate(a, mapWidth, SetObjects.getHeight()), useMirrorFitness);
        else
        {
            SetObjects.setMap(deflatten(a, mapWidth, SetObjects.getHeight()), useMirrorFitness);
        }
        MainMenuNavigation.changeSceneIndex(5);
    }

    int[,] templateDeflatten(Gene[] arrays, int width, int height)
    {
        int[,] result = new int[height, width * (useMirrorFitness ? 2 : 1)];
        int[,] currTemplate;
        for (int i = 0; i < height / 5; i++)
        {
            for (int j = 0; j < width / 5; j++)
            {
                currTemplate = PossibleTemplates.getTemplate((int)((Gene)arrays[i * (width / 5) + j]).Value);
                for (int k = 0; k < 5; k++)
                {
                    for (int l = 0; l < 5; l++)
                    {
                        result[5 * i + k, 5 * j + l] = currTemplate[k, l];
                        if (useMirrorFitness)
                            result[5 * i + k, (width * 2) - 1 - (5 * j + l)] = currTemplate[k, l];
                    }
                }
            }
        }
        return result;
    }

    //Mungkin kapan kapan aja di tambahi Dilation + erosion
    int[,] putPlayerinTemplate(Gene[] arrays, int width, int height)
    {
        int[,] tempTemplate = templateDeflatten(arrays, width, height);
        Coordinate[] _5Coordinates = new Coordinate[] {
            new Coordinate(Mathf.RoundToInt(width/3),Mathf.RoundToInt(height/4)),
            new Coordinate(Mathf.RoundToInt((width*2)/3),Mathf.RoundToInt(height/4)),
            new Coordinate(Mathf.RoundToInt(width/2),Mathf.RoundToInt(height/2)),
            new Coordinate(Mathf.RoundToInt(width/3),Mathf.RoundToInt((height*3)/4)),
            new Coordinate(Mathf.RoundToInt((width*2)/3),Mathf.RoundToInt((height*3)/4)),
        };
        foreach (Coordinate item in _5Coordinates)
        {
            if (tempTemplate[item.yCoor, item.xCoor] == 0)
                tempTemplate[item.yCoor, item.xCoor] = 3;
            else
            {
                Queue<Coordinate> q = new Queue<Coordinate>();
                Coordinate currentCoor,tempCoor;
                q.Enqueue(item);
                while (q.Count != 0)
                {
                    currentCoor = q.Dequeue();
                    for (int k = 0; k < 4; k++)
                    {
                        tempCoor = new Coordinate(currentCoor.xCoor + Mathf.RoundToInt(Mathf.Sin(k * Mathf.PI / 2)), currentCoor.yCoor + Mathf.RoundToInt(Mathf.Cos(k * Mathf.PI / 2)));
                        q.Enqueue(tempCoor);
                        if (tempTemplate[tempCoor.yCoor,tempCoor.xCoor] == 0)
                        {
                            tempTemplate[item.yCoor, item.xCoor] = 3;
                            q.Clear();
                            break;
                        }
                    }
                }
            }
        }
        return tempTemplate;
    }

    int[,] deflatten(Gene[] arrays, int width, int height)
    {
        int[,] result = new int[height, width * (useMirrorFitness ? 2 : 1)];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //Debug.Log(i + "," + j+","+ (width - 1 - j));
                result[i, j] = (int)((Gene)arrays[i * width + j]).Value;
                if (useMirrorFitness)
                    result[i, (width * 2) - 1 - j] = (int)((Gene)arrays[i * width + j]).Value;
            }
        }
        return result;
    }

    double fitnessFunction(int[,] map)
    {
        int playerAmount = 0;
        InLoopFitnessBase[] currentFitnesses = (InLoopFitnessBase[])fitnesses.Clone();
        double[] fitnessScores = new double[fitnesses.Length];
        foreach (InLoopFitnessBase item in currentFitnesses)
        {
            item.resetVariables();
        }

        Coordinate tempCoor;
        for (int i = 0; i < SetObjects.getHeight(); i++)
            for (int j = 0; j < mapWidth; j++) {
                tempCoor = new Coordinate(j, i);
                if (map[tempCoor.yCoor, tempCoor.xCoor] == 3)
                    playerAmount++;
                foreach (InLoopFitnessBase item in currentFitnesses)
                {
                    item.calculateFitness(map,tempCoor);
                }
            }

        for (int i = 0; i < currentFitnesses.Length; i++)
        {
            fitnessScores[i] = currentFitnesses[i].getFitnessScore();
        }

        //Debug.Log(printFitness(currentFitnesses));
        //Debug.Log(String.Join(" - ", fitnessScores));
        return Mathf.Pow((float)fitnessScores.Sum() / Mathf.Pow(MathF.Abs(playerAmount - 10) * 5 + 1, 3), 3);
    }

    public static string printFitness(FitnessBase[] fitnesses)
    {
        string s = "";
        for (int i = 0; i < fitnesses.Length; i++)
        {
            s += fitnesses[i].FitnessName + "\t: " + fitnesses[i].getFitnessScore() + "\n";
        }
        return s;
    }

    public static string print2DArray(int[,] array)
    {
        string s = "";
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                s += array[i, j] + ",";
            }
            s += "\n";
        }
        return s;
    }

}