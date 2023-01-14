using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SetObjects : MonoBehaviour
{
    static int width;
    static int height;
    static int[,] stage;
    //0 = Kosong, 1 = batu, 2 = powerup, 3 = character

    [SerializeField] GameObject levelCamera;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject enemyPrefab;
    public static void initializeSize(int w, int h)
    {
        height = h;
        width = w;
        if (w % 2 == 1)
            width--;
        stage = new int[h - 2, (w / 2) - 1];
    }

    static (int, int) getrandomcoordinate()
    {
        return (Mathf.RoundToInt(Random.Range(0, (width - 2) / 2)) + 1, -Mathf.RoundToInt(Random.Range(0, height - 2)) - 1);
    }

    public static void setStage(int[,] stagearray)
    {
        stage = stagearray;
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
            mapTilemap.SetTile(new Vector3Int(i, -height + 1, 1), rok);
        }
        //vertikal
        for (int i = 1; i < height - 1; i++)
        {
            mapTilemap.SetTile(new Vector3Int(0, -i, 1), rok);
            mapTilemap.SetTile(new Vector3Int(width - 1, -i, 1), rok);
        }

        //buat coba naruh orang
        int x, y;
        (x, y) = getrandomcoordinate();
        playerPrefab = Instantiate(playerPrefab, new Vector3(x, y), Quaternion.identity);
        levelCamera.GetComponent<CameraController2D>().setCameraFollower(playerPrefab);
        GameObject tempEnemyPrefab = Instantiate(enemyPrefab, new Vector3(width - x, y), Quaternion.identity);
        tempEnemyPrefab.GetComponent<SnowBrawler>().initializeBrawler(false, 5);
        tempEnemyPrefab.GetComponent<SpriteRenderer>().color = Color.magenta;
        for (int i = 1; i < 5; i++)
        {
            (x, y) = getrandomcoordinate();
            tempEnemyPrefab = Instantiate(enemyPrefab, new Vector3(x, y), Quaternion.identity);
            tempEnemyPrefab.GetComponent<SnowBrawler>().initializeBrawler(true, i);
            tempEnemyPrefab = Instantiate(enemyPrefab, new Vector3(width - x, y), Quaternion.identity);
            tempEnemyPrefab.GetComponent<SnowBrawler>().initializeBrawler(false, i + 5);
            tempEnemyPrefab.GetComponent<SpriteRenderer>().color = Color.magenta;
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
