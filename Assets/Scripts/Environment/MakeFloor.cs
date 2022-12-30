using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MakeFloor : MonoBehaviour
{
    Tilemap mapTilemap;
    public TileBase rok;
    public int xLength, yHeight;
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
