using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SetStones : MonoBehaviour
{
    static int width;
    static int height;

    public static void initializeSize(int w, int h)
    {
        height = h;
        width = w;
        if (w % 2 == 1)
            width--;
    }
    public static int getWidth()
    {
        return width;
    }
    public static int getHeight()
    {
        return height;
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
            mapTilemap.SetTile(new Vector3Int(i, -height+1, 1), rok);
        }
        //vertikal
        for (int i = 1; i < height-1; i++)
        {
            mapTilemap.SetTile(new Vector3Int(0, -i, 1), rok);
            mapTilemap.SetTile(new Vector3Int(width-1, -i, 1), rok);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
