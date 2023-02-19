using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MaptoArray : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Tilemap tilemap = GetComponent<Tilemap>();

        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        Debug.Log(allTiles.Length+","+bounds.size.y+","+bounds.size.x);
        int[,] map = new int[bounds.size.y-2, bounds.size.x-2];

        for (int x = 1; x < bounds.size.x - 1; x++)
        {
            for (int y = 1; y < bounds.size.y - 1; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null && tile.name == "rok")
                {
                    map[bounds.size.y - y - 2, x - 1] = 1;
                }
            }
        }

        SetObjects.setMap(map, false);
    }
}
