using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using Random = UnityEngine.Random;
using UnityEngine.TextCore.Text;
using System.Linq;

public class AStarAlgorithm : MonoBehaviour
{
    public static float circleSize = 0.4f;

    public static Coordinate[] makeWay(Transform character, Transform ball)
    {
        return makeWay(Coordinate.returnAsCoordinate(character.position), Coordinate.returnAsCoordinate(ball.position));
    }
    public static Coordinate[] makeWay(Coordinate character, Coordinate ball)
    {
        // layermask bukan integer tapi biner ternyata
        Vector2 arah = (ball.returnAsVector() - character.returnAsVector());
        arah = arah.normalized;
        float dist = Vector3.Distance(ball.returnAsVector(), character.returnAsVector());
        if (!Physics2D.CircleCast(character.returnAsVector(), circleSize, arah, dist, 64))
        {
            Coordinate ints = vectorToCoordinate(ball.returnAsVector());
            //Debug.Log("Shortcut");
            return new Coordinate[] { ints };
        }
        else
        {
            AstarNode result = doAstarAlgo(character, ball, SetObjects.getMap(false));
            if (result == null)
                return null;
            ArrayList coordinates = new ArrayList();
            while (result.parentNode != null)
            {
                coordinates.Add(result.coordinate);
                result = result.parentNode;
            }
            coordinates.Reverse();
            //Debug.Log("Panjang : " + coordinates.Count);
            //string tes1 = "";
            //foreach (Coordinate item in coordinates)
            //{
            //    tes1 += item.ToString() + "\n";
            //}
            //Debug.Log(tes1);
            Coordinate[] finalcoordinates = optimizePath((Coordinate[])coordinates.ToArray(typeof(Coordinate)));
            //Debug.Log("Panjang Optimized : " + finalcoordinates.Length);
            //tes1 = "";
            //foreach (Coordinate item in finalcoordinates)
            //{
            //    tes1 += item.ToString() + "\n";
            //}
            //Debug.Log(tes1);
            return finalcoordinates;
        }
    }

    static Coordinate[] optimizePath(Coordinate[] path)
    {
        List<Coordinate> resultAL = new List<Coordinate>();
        Coordinate currentPoint = path[0],checkedPoint = path[0];
        int pathIndex = 1;
        Vector2 arah ;
        float dist;
        resultAL.Add(currentPoint);
        while (pathIndex < path.Length - 1)
        {
            arah = (path[pathIndex].returnAsVector() - currentPoint.returnAsVector()).normalized;
            dist = Vector2.Distance(path[pathIndex].returnAsVector(), currentPoint.returnAsVector());
            if (Physics2D.CircleCast(currentPoint.returnAsVector(), circleSize, arah, dist, 64))
            {
                resultAL.Add(path[pathIndex-1]);
                currentPoint = checkedPoint;
            }
            checkedPoint = path[pathIndex];
            pathIndex++;
        }
        resultAL.Add(path[path.Length - 1]);
        return resultAL.ToArray();
    }

    //Fix bisa
    public static AstarNode doAstarAlgo(Transform character, Transform ball)
    {
        Coordinate posisikarakter = vectorToCoordinate(character.position);
        Coordinate posisibola = vectorToCoordinate(ball.position);
        //inisialisasi
        int[,] map = SetObjects.getMap(false);
        return doAstarAlgo(posisikarakter, posisibola, map);
    }

    public static AstarNode doAstarAlgo(Coordinate posisikarakter, Coordinate posisibola, int[,] map)
    {

        bool[,] isChecked = new bool[map.GetLength(0), map.GetLength(1)];
        ArrayList listNode = new ArrayList
        {
            new AstarNode(posisikarakter, 0, Coordinate.Distance(posisikarakter, posisibola), null)
        };
        AstarNode currentnode, tempnode;
        float distance;
        bool isput,inBounds;
        int mapheight = map.GetLength(0), maplength = map.GetLength(1);
        Coordinate newcoor;
        //Debug.Log($"Mulai debug Astar dari {posisikarakter} ke {posisibola}");
        while (listNode.Count > 0)
        {
            currentnode = (AstarNode)listNode[0];
            isChecked[currentnode.coordinate.yCoor, currentnode.coordinate.xCoor] = true;
            listNode.RemoveAt(0);
            if (currentnode.coordinate.xCoor == posisibola.xCoor && currentnode.coordinate.yCoor == posisibola.yCoor)
            {
                return currentnode;
            }
            for (int i = 0; i < 4; i++)
            {
                newcoor = new Coordinate(currentnode.coordinate.xCoor + Mathf.RoundToInt(Mathf.Sin(i * Mathf.PI / 2)), currentnode.coordinate.yCoor + Mathf.RoundToInt(Mathf.Cos(i * Mathf.PI / 2)));
                inBounds = newcoor.yCoor >= 0 && newcoor.yCoor < mapheight && newcoor.xCoor >= 0 && newcoor.xCoor < maplength;
                if (inBounds && map[newcoor.yCoor, newcoor.xCoor] != 1 && !isChecked[newcoor.yCoor, newcoor.xCoor])
                {
                    distance = Coordinate.Distance(newcoor, posisibola);
                    tempnode = new AstarNode(newcoor, currentnode.g + 1, distance, currentnode);

                    isput = false;
                    for (int j = 0; j < listNode.Count; j++)
                    {
                        if (((AstarNode)listNode[j]).f >= tempnode.f)
                        {
                            isChecked[newcoor.yCoor, newcoor.xCoor] = true;
                            isput = true;
                            listNode.Insert(j, tempnode);
                            break;
                        }
                    }
                    if (!isput)
                        listNode.Add(tempnode);
                }
            }
        }
        Debug.Log("Null : " + posisikarakter + " ke " + posisibola);
        return null;
    }

        public static Coordinate vectorToCoordinate(Vector2 vent)
    {
        return new Coordinate(Mathf.RoundToInt(vent.x - 1.5f), Mathf.RoundToInt(-vent.y - 0.5f));
    }
}

public class AstarNode
{
    [SerializeField]
    public Coordinate coordinate;
    public float f, g, h;
    public AstarNode parentNode;
    //g = jarak total ditempuh dari karakter ke lokasi
    //h = Heuristic
    //f = total keduanya (semakin kecil semakin bagus)

    public AstarNode(Coordinate coordinate, float g, float h,AstarNode parent)
    {
        this.coordinate = coordinate;
        this.g = g;
        this.h = h;
        this.f = g + h;
        this.parentNode = parent;
    }

    public override string ToString()
    {
        return coordinate.ToString() + ", G= " + g + ", H= " + h + ", F= " + f + " Parent =" + (parentNode != null);
    }
}

[System.Serializable, Inspectable]
public class Coordinate
{
    [SerializeField]
    public int xCoor, yCoor;

    public Coordinate(int xCoor, int yCoor)
    {
        this.xCoor = xCoor;
        this.yCoor = yCoor;
    }

    public static float Distance(Coordinate c1, Coordinate c2)
    {
        return Mathf.Sqrt(Mathf.Pow(c1.xCoor - c2.xCoor, 2) + Mathf.Pow(c1.yCoor - c2.yCoor, 2));
    }

    public static Coordinate returnAsCoordinate(Vector2 vector)
    {
        return new Coordinate(Mathf.RoundToInt(vector.x - 1.5f), Mathf.RoundToInt(-vector.y - 0.5f));
    }

    public Vector2 returnAsVector()
    {
        return new Vector2(xCoor + 1.5f, -yCoor - 0.5f);
    }

    public override string ToString()
    {
        return "X : "+xCoor + ", Y : " + yCoor;
    }

    public static Coordinate getRandomCoordinate()
    {
        return new Coordinate(Mathf.RoundToInt(Random.Range(0, SetObjects.getWidth()  / 2)), Mathf.RoundToInt(Random.Range(0, SetObjects.getHeight())));
    }
}