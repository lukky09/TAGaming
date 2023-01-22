using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAlgorithm : MonoBehaviour
{
    public static int[,] makeWay(Transform character, Transform ball)
    {
        int[,] map = SetObjects.getMap(false);
        RaycastHit2D raycast = Physics2D.Linecast(character.localPosition, ball.localPosition);
        if (raycast.collider == null)
        {
            int[] ints = vectorToCoordinate(ball.localPosition);
            return new int[,] { { ints[0], ints[1] } };
        }
        else
        {
            ArrayList jalan = new ArrayList();
            int[] posisikarakter = vectorToCoordinate(character.position);
            int[] posisibola = vectorToCoordinate(ball.position);
            return null;
        }
    }



    // 0 selalu y, 1 selalu x
    public static int[] vectorToCoordinate(Vector2 vent)
    {
        return new int[] { Mathf.RoundToInt(-vent.y + 0.5f), Mathf.RoundToInt(vent.x - 1.5f) };
    }

    public static Vector2 coordinatetoVector(int x, int y)
    {
        return new Vector2(x + 1.5f, -y - 0.5f);
    }
}
class AstarNode
{
    int xCoor, yCoor, f, g, h;
    AstarNode parentNode;
    //g = jarak total ditempuh dari karakter ke lokasi
    //h = Heuristic
    //f = total keduanya (semakin kecil semakin bagus)

    public AstarNode(int xCoor, int yCoor, int g, int h,AstarNode parent)
    {
        this.xCoor = xCoor;
        this.yCoor = yCoor;
        this.g = g;
        this.h = h;
        this.f = g + h;
        this.parentNode = parent;
    }
}

