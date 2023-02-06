using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MakeFloor : MonoBehaviour
{
    Tilemap mapTilemap;
    public TileBase floor;
    // Start is called before the first frame update
    void Start()
    {
        mapTilemap = this.GetComponent<Tilemap>();
        mapTilemap.ClearAllTiles();
        TileBase[] tilemaps = new TileBase[(SetObjects.getHeight() + 2) * (SetObjects.getWidth() + 2)];
        Array.Fill(tilemaps, floor);
        mapTilemap.SetTilesBlock(new BoundsInt(new Vector3Int(0, -SetObjects.getHeight() - 1, 1), new Vector3Int(SetObjects.getWidth() + 2, SetObjects.getHeight() + 2, 1)), tilemaps);
    }

}
