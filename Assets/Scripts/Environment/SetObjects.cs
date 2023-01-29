using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SetObjects : MonoBehaviour
{
    static int width;
    static int height;
    static int[,] stageFolded;
    static int[,] stageUnfolded;
    //0 = Kosong, 1 = batu, 2 = powerup, 3 = character, 4 = snowball (khusus unfolded)

    public static void initializeSize(int w, int h)
    {
        height = h;
        width = w;
        if (w % 2 == 1)
            width--;
        stageFolded = new int[h - 2, (w / 2) - 1];
        stageUnfolded = new int[h - 2, w - 2];
    }

    public static void setStage(int[,] stagearray)
    {
        stageFolded = stagearray;
    }

    public static void setMap(int index1,int index2, int number)
    {
        stageUnfolded[index1,index2] = number;
    }

    public static int getWidth()
    {
        return width;
    }
    public static int getHeight()
    {
        return height;
    }
    public static int[,] getMap(bool folded)
    {
        if (folded)
            return stageFolded;
        else
            return stageUnfolded;
    }

    Tilemap mapTilemap;
    public TileBase rok;
    // Start is called before the first frame update
    void Start()
    {
        mapTilemap = this.GetComponent<Tilemap>();
        //horizontal
        for (int i = 0; i < width; i++)
        {
            mapTilemap.SetTile(new Vector3Int(i, 0, 1), rok);
            mapTilemap.SetTile(new Vector3Int(i, -height + 1, 1), rok);
        }
        //vertikal
        for (int i = 1; i < height - 1; i++)
        {
            mapTilemap.SetTile(new Vector3Int(0, -i, 1), rok);
            mapTilemap.SetTile(new Vector3Int(width - 1, -i, 1), rok);
        }

    }

}
