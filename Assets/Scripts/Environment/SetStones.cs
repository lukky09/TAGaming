using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SetStones : MonoBehaviour
{
    static int width;
    static int height;

    public static void initializeSize(int h, int w)
    {
        height = h;
        width = w;
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
        for (int i = 0; i < 10; i++)
        {
            mapTilemap.SetTile(new Vector3Int(i, i, 1), rok);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
