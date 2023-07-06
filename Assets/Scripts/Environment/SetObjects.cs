using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SetObjects : MonoBehaviour
{
    static int width;
    static int height;
    static int[,] stageFolded;
    static int[,] stageUnfolded;
    //0 = Kosong, 1 = batu, 2 = powerup, 3 = character, 4 = snowball (khusus unfolded)
    Tilemap mapTilemap;
    public TileBase rok;
    public GameObject powerUpContainer;
    public GameObject powerUp;
    [SerializeField] GameObject playerManagerReference;
    [SerializeField] bool only1BotActive;
    [SerializeField] ColorManager colManager;

    public static void initializeSize(int w, int h)
    {
        height = h;
        width = w;
        if (w % 2 == 1)
            width--;
    }

    public static void setMap(int[,] stagearray, bool isFolded)
    {
        //mapFolded itu asumsikan di kiri
        if (isFolded)
        {
            stageFolded = stagearray;
            stageUnfolded = new int[height - 2, width - 2];
            for (int i = 0; i < height - 2; i++)
            {
                for (int j = 0; j < (int)((width - 2) / 2); j++)
                {
                    stageUnfolded[i, j] = stageFolded[i, j];
                    stageUnfolded[i, width - 3 - j] = stageFolded[i, j];
                }
            }
        }
        else
        {
            stageUnfolded = stagearray;
            height = stagearray.GetLength(0) + 2;
            width = stagearray.GetLength(1) + 2;
        }
    }

    public static void setMap(int index1, int index2, int number)
    {
        stageUnfolded[index1, index2] = number;
    }


    public static int getWidth()
    {
        return width - 2;
    }
    public static int getHeight()
    {
        return height - 2;
    }
    public static int[,] getMap(bool folded)
    {
        if (folded)
            return stageFolded;
        else
            return stageUnfolded;
    }


    // Start is called before the first frame update
    void Start()
    {
        mapTilemap = this.GetComponent<Tilemap>();
        fillMap();
    }

    public void fillMap()
    {
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
        Coordinate tempCoor;
        GameObject temp;
        bool playerMade = false, oneBotAI = false;
        for (int i = 0; i < height - 2; i++)
            for (int j = 0; j < width - 2; j++)
            {
                tempCoor = new Coordinate(j, i);
                if (stageUnfolded[i, j] == 1)
                    mapTilemap.SetTile(new Vector3Int(j + 1, -i - 1, 1), rok);
                else if (stageUnfolded[i, j] == 2)
                {
                    temp = Instantiate(powerUp, tempCoor.returnAsVector(), Quaternion.identity);
                    temp.transform.SetParent(powerUpContainer.transform);
                }
                else if (stageUnfolded[i, j] == 3)
                {
                    if (!playerMade)
                    {
                        playerManagerReference.GetComponent<PlayersManager>().makeNewPlayer(tempCoor);
                        playerMade = true;
                    }
                    else if (only1BotActive && !oneBotAI)
                    {
                        playerManagerReference.GetComponent<PlayersManager>().makeNewBot(tempCoor, j < (int)(width / 2) + 1);
                        oneBotAI = true;
                    }
                }

            }
    }

    public void clearMap()
    {
        mapTilemap.ClearAllTiles();
    }

}
