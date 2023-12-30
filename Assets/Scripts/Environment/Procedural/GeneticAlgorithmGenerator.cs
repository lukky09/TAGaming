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
using System.IO;
using Debug = UnityEngine.Debug;

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
    [SerializeField] MainMenuNavigation MMN;
    [SerializeField] bool useTemplatedGeneration;
    [SerializeField] bool bestFitnessDebug;
    [SerializeField] bool printLevelsFitness;
    [SerializeField] int StagnationTerminationAmt;
    InLoopFitnessBase[] fitnesses;

    private void Start()
    {
        StartCoroutine(delayedStart());
    }

    IEnumerator delayedStart()
    {
        yield return new WaitForSeconds(1f);
        algorithmStart();
    }

    void algorithmStart()
    {
        fitnesses = GetComponents<InLoopFitnessBase>();
        useTemplatedGeneration = MainMenuNavigation.isTemplate;
        for (int i = 0; i < fitnesses.Length; i++)
        {
            //Debug.Log(!fitnesses[i].IsUsed +" - "+ (useTemplatedGeneration && !fitnesses[i].ForTemplateGen) + " - " + (!useTemplatedGeneration && !fitnesses[i].ForTileGen));
            //Ngilangin Fitness yang ngga dipakai
            if (!fitnesses[i].IsUsed || (useTemplatedGeneration && !fitnesses[i].ForTemplateGen) || (!useTemplatedGeneration && !fitnesses[i].ForTileGen))
                fitnesses[i] = null;
        }
        fitnesses = fitnesses.Where(f => f != null).ToArray();

        double fitness;

        int generatedMapLength = SetObjects.getHeight() * SetObjects.getWidth() / 2;

        ChromosomeBase chromosome;
        //Kromosom
        if (useTemplatedGeneration)
            chromosome = new TemplatedMapChromosome(Mathf.RoundToInt(generatedMapLength / 25));
        else
        {
            chromosome = new GameChromosome(generatedMapLength, Mathf.FloorToInt(GetComponent<PowerUpRatioFitness>().getRatio() * generatedMapLength));
        }

        //Populasi
        var population = new Population(50, 100, chromosome);
        //Fitness
        var fitnessfunc = new FuncFitness((c) =>
        {
            var fc = c.GetGenes();
            int[,] map;
            //Menghasilkan map utuh yang siap diperiksa oleh fitness
            if (useTemplatedGeneration)
                map = putPlayerinTemplate(templateDeflatten(fc, SetObjects.getWidth() / 2, SetObjects.getHeight()), SetObjects.getWidth() / 2, SetObjects.getHeight());

            else
                map = deflatten(fc, SetObjects.getWidth() / 2, SetObjects.getHeight());
            fitness = fitnessFunction(map, fc);
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

        StreamWriter stream;
        string path = @"C:\Users\gamel\Desktop\File TA\nilai.txt";

        var fs = new FileStream(path, System.IO.FileMode.Open);
        stream = new StreamWriter(fs);
        if (printLevelsFitness)
        {
            stream.WriteLine("Population,Fitness");
            ga.GenerationRan += (sender, e) =>
            {
                if (useTemplatedGeneration)
                {
                    var currentBestChromosome = ga.BestChromosome as TemplatedMapChromosome;
                    stream.WriteLine($"{ga.GenerationsNumber},{currentBestChromosome.Fitness.Value}");
                }
                else
                {
                    var currentBestChromosome = ga.BestChromosome as GameChromosome;
                    stream.WriteLine($"{ga.GenerationsNumber},{currentBestChromosome.Fitness.Value}");
                }
            };
        }

        ga.Start();

        if (printLevelsFitness)
            stream.Close();

        var a = ga.BestChromosome.GetGenes();
        if (bestFitnessDebug)
        {
            var fc = ga.BestChromosome.GetGenes();
            int[,] map;
            //Menghasilkan map utuh yang siap diperiksa oleh fitness
            if (useTemplatedGeneration)
                map = putPlayerinTemplate(templateDeflatten(fc, SetObjects.getWidth() / 2, SetObjects.getHeight()), SetObjects.getWidth() / 2, SetObjects.getHeight());

            else
                map = deflatten(fc, SetObjects.getWidth() / 2, SetObjects.getHeight());

            int playerAmount = 0;
            InLoopFitnessBase[] currentFitnesses = (InLoopFitnessBase[])fitnesses.Clone();

            //Khusus Template Variety
            for (int i = 0; i < currentFitnesses.Length; i++)
                if (currentFitnesses[i].GetType() == GetComponent<TemplateVarietyFitness>().GetType())
                    ((TemplateVarietyFitness)currentFitnesses[i]).getTemplateMap(fc);


            double[] fitnessScores = new double[fitnesses.Length];
            foreach (InLoopFitnessBase item in currentFitnesses)
                item.resetVariables();

            Coordinate tempCoor;
            for (int i = 0; i < SetObjects.getHeight(); i++)
                for (int j = 0; j < SetObjects.getWidth(); j++)
                {
                    tempCoor = new Coordinate(j, i);
                    if (map[tempCoor.yCoor, tempCoor.xCoor] == 3)
                        playerAmount++;
                    foreach (InLoopFitnessBase item in currentFitnesses)
                    {
                        item.calculateFitness(map, tempCoor);
                    }
                }

            Debug.Log("Best Fitness : \n" + printFitness(currentFitnesses));
        }
        if (useTemplatedGeneration)
        {
            SetObjects.setMap(putPlayerinTemplate(templateDeflatten(a, SetObjects.getWidth() / 2, SetObjects.getHeight()), SetObjects.getWidth() / 2, SetObjects.getHeight()), useMirrorFitness);
        }
        else
        {
            //Debug.Log(print2DArray(deflatten(a, SetObjects.getWidth() / 2, SetObjects.getHeight())));
            SetObjects.setMap(deflatten(a, SetObjects.getWidth() / 2, SetObjects.getHeight()), useMirrorFitness);
        }

        //Cek kalau multiplayer
        if (LobbyManager.instance != null && LobbyManager.instance.IsOnline )
        {
            LobbyManager.instance.changeLobbyVariable(
                new string[] { "MapSize", "MapData" },
                new string[] { $"{SetObjects.getWidth()},{SetObjects.getHeight()}", geneToMultiplayerData(a) });
            //printMapOnlineEndcoding(a, SetObjects.getMap(false));
            MMN.changeSceneIndex(-8);
            return;
        }
        MMN.changeSceneIndex(-6);
    }

    double fitnessFunction(int[,] map, Gene[] original)
    {
        int playerAmount = 0;
        InLoopFitnessBase[] currentFitnesses = (InLoopFitnessBase[])fitnesses.Clone();

        //Khusus Template Variety
        for (int i = 0; i < currentFitnesses.Length; i++)
            if (currentFitnesses[i].GetType() == GetComponent<TemplateVarietyFitness>().GetType())
                ((TemplateVarietyFitness)currentFitnesses[i]).getTemplateMap(original);


        double[] fitnessScores = new double[fitnesses.Length];
        foreach (InLoopFitnessBase item in currentFitnesses)
            item.resetVariables();

        Coordinate tempCoor;
        for (int i = 0; i < SetObjects.getHeight(); i++)
            for (int j = 0; j < SetObjects.getWidth(); j++)
            {
                tempCoor = new Coordinate(j, i);
                if (map[tempCoor.yCoor, tempCoor.xCoor] == 3)
                    playerAmount++;
                foreach (InLoopFitnessBase item in currentFitnesses)
                {
                    item.calculateFitness(map, tempCoor);
                }
            }

        for (int i = 0; i < currentFitnesses.Length; i++)
        {
            fitnessScores[i] = currentFitnesses[i].getFitnessScore();
        }

        //Karena map template tidak ada orang
        if (useTemplatedGeneration)
            return Mathf.Pow((float)fitnessScores.Sum(), 3);
        else
            return Mathf.Pow((float)fitnessScores.Sum() / Mathf.Pow(MathF.Abs(playerAmount - 10) * 5 + 1, 3), 3);
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


    string geneToMultiplayerData(Gene[] arrays)
    {
        string result = "";
        foreach (Gene eh in arrays)
            result += eh.Value.ToString() + (useTemplatedGeneration ? " " : "");
        return result;
    }

    public static int[,] multiplayerDataToMap(string MapData, int width, int height)
    {
        int[,] result = new int[height, width / 2];
        // Kalau Generation biasa
        if (!MapData.Contains(' '))
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width/2; j++)
                {
                    try
                    {
                        result[i, j] = (int)Char.GetNumericValue(MapData[i * (width / 2) + j]);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(i * (width / 2) + j +" , "+ MapData.Length);
                    }
                    
                }
        //Kalau Template Generation
        else
        {
            string[] currentMapData = MapData.Split(' ');
            int[,] currTemplate;
            for (int i = 0; i < height / 5; i++)
            {
                for (int j = 0; j < width / 10; j++)
                {
                    currTemplate = PossibleTemplates.getTemplate(Int32.Parse(currentMapData[i * (width / 10) + j]));
                    for (int k = 0; k < 5; k++)
                    {
                        for (int l = 0; l < 5; l++)
                        {
                            result[5 * i + k, 5 * j + l] = currTemplate[k, l];
                        }
                    }
                }
            }
            result = putPlayerinTemplate(result, width / 2, height);
        }
        return result;
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
    static int[,] putPlayerinTemplate(int[,] arrays, int width, int height)
    {
        int[,] tempTemplate = arrays;
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
                Coordinate currentCoor, tempCoor;
                q.Enqueue(item);
                while (q.Count != 0)
                {
                    currentCoor = q.Dequeue();
                    for (int k = 0; k < 4; k++)
                    {
                        tempCoor = new Coordinate(currentCoor.xCoor + Mathf.RoundToInt(Mathf.Sin(k * Mathf.PI / 2)), currentCoor.yCoor + Mathf.RoundToInt(Mathf.Cos(k * Mathf.PI / 2)));
                        q.Enqueue(tempCoor);
                        if (tempTemplate[tempCoor.yCoor, tempCoor.xCoor] == 0)
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
                if (array[i, j] == 1)
                    s += "<color=grey>" + array[i, j] + "</color>,";
                else if (array[i, j] == 2)
                    s += "<color=green>" + array[i, j] + "</color>,";
                else if (array[i, j] == 3)
                    s += "<color=red>" + array[i, j] + "</color>,";
                else
                    s += array[i, j] + ",";
            }
            s += "\n";
        }
        return s;
    }

    void printMapOnlineEndcoding(Gene[] geneOriginal, int[,] array)
    {
        string encoding = geneToMultiplayerData(geneOriginal);
        Debug.Log("Sebelum Encoding:");
        Debug.Log(print2DArray(array));
        Debug.Log("Saat Encoding:");
        Debug.Log(encoding);
        Debug.Log("Setelah Encoding:");
        Debug.Log(print2DArray(multiplayerDataToMap(encoding, SetObjects.getWidth(), SetObjects.getHeight())));
    }
}