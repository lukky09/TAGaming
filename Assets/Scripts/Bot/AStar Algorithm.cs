using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class AStarAlgorithm : MonoBehaviour
{
    public static Coordinate[] makeWay(Transform character, Transform ball)
    { 
        RaycastHit2D raycast = Physics2D.Linecast(character.localPosition, ball.localPosition);
        if (raycast.collider == null)
        {
            Coordinate ints = vectorToCoordinate(ball.localPosition);
            return new Coordinate[] { ints };
        }
        else
        {
            AstarNode result = doAstarAlgo(character, ball);
            ArrayList coordinates = new ArrayList();
            while (result.parentNode != null)
            {
                coordinates.Add(result.coordinate);
                result = result.parentNode;
            }
            coordinates.Reverse();
            Coordinate[] finalcoordinates = (Coordinate[])coordinates.ToArray(typeof(Coordinate));
            return optimizePath(finalcoordinates);
        }
    }

    static Coordinate[] optimizePath(Coordinate[] path)
    {
        ArrayList resultAL = new ArrayList();
        Coordinate currentpoint = path[0];
        resultAL.Add(currentpoint);
        int tempindex = 1;
        while (tempindex <= path.Length - 1)
        {
            if (!isStraight(currentpoint, path[tempindex]) && Physics2D.Linecast(currentpoint.returnAsVector(), path[tempindex].returnAsVector()))
            {
                resultAL.Add(path[tempindex - 1]);
                currentpoint = path[tempindex - 1];
                tempindex--;
            }
            tempindex++;
        }
        resultAL.Add(path[1]);
        return (Coordinate[])resultAL.ToArray(typeof(Coordinate));
    }

    static bool isStraight(Coordinate c1, Coordinate c2)
    {
        return ((c1.xCoor - c2.xCoor) * (c1.yCoor - c2.yCoor) == 0);
    }

    //Fix bisa
    static AstarNode doAstarAlgo(Transform character, Transform ball)
    {
        int[,] map = SetObjects.getMap(false);
        ArrayList listNode = new ArrayList();
        Coordinate posisikarakter = vectorToCoordinate(character.position);
        Coordinate posisibola = vectorToCoordinate(ball.position);
        //inisialisasi
        listNode.Add(new AstarNode(posisikarakter, 0, Vector2.Distance(character.position, ball.position), null));

        AstarNode currentnode, tempnode, resultnode = null;
        float distance;
        bool isput;
        int mapheight = map.GetLength(0), maplength = map.GetLength(1);
        Coordinate newcoor;
        while (listNode.Count > 0)
        {
            currentnode = (AstarNode)listNode[0]; 
            //Debug.Log(currentnode.ToString());
            listNode.RemoveAt(0);
            if (currentnode.coordinate.xCoor == posisibola.xCoor && currentnode.coordinate.yCoor == posisibola.yCoor)
            {
                resultnode = currentnode;
                break;
            }
            for (int i = 0; i < 4; i++)
            {
                newcoor = new Coordinate(currentnode.coordinate.xCoor + Mathf.RoundToInt(Mathf.Sin(i * Mathf.PI / 2)), currentnode.coordinate.yCoor + Mathf.RoundToInt(Mathf.Cos(i * Mathf.PI / 2)));
                if (newcoor.yCoor < mapheight && newcoor.xCoor < maplength && map[newcoor.yCoor, newcoor.xCoor] != 1)
                {
                    distance = Vector2.Distance(newcoor.returnAsVector(), ball.position);
                    tempnode = new AstarNode(newcoor, currentnode.g + 1, distance, currentnode);
                    
                    isput = false;
                    for (int j = 0; j < listNode.Count; j++)
                    {
                        if (((AstarNode)listNode[j]).f >= tempnode.f)
                        {
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
        return resultnode;
    }

    public static Coordinate vectorToCoordinate(Vector2 vent)
    {
        return new Coordinate(Mathf.RoundToInt(vent.x - 1.5f), Mathf.RoundToInt(-vent.y - 0.5f));
    }
}
class AstarNode
{
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
        return coordinate.ToString() + ", G= " + g + ", H= " + h + ", Parent =" + (parentNode != null);
    }
}

[Inspectable]
public class Coordinate
{
    public int xCoor, yCoor;

    public Coordinate(int xCoor, int yCoor)
    {
        this.xCoor = xCoor;
        this.yCoor = yCoor;
    }

    public Vector2 returnAsVector()
    {
        return new Vector2(xCoor + 1.5f, -yCoor - 0.5f);
    }

    
    public override string ToString()
    {
        return xCoor + "," + yCoor;
    }
}