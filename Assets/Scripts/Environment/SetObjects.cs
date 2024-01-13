using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Mathematics;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;

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
    PlayerManager playerManagerReference;
    [SerializeField] GameObject _RTCManagerReference;

    public Coordinate[,] PlayerCoordinates;
    public int[,] PlayerPositions;

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
        PlayerCoordinates = new Coordinate[2, 5];
        mapTilemap = this.GetComponent<Tilemap>();
        GameObject barScoreGO = Instantiate(_RTCManagerReference);
        if(!LobbyManager.IsOnline || LobbyManager.instance.IsHosting)
            barScoreGO.GetComponent<NetworkObject>().Spawn(true);
        playerManagerReference = barScoreGO.GetComponent<PlayerManager>();
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
        int[] playerAmt = new int[] { 0, 0 };
        int k;
        PlayerPositions = new int[,] { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 } };
        for (int i = 0; i < height - 2; i++)
            for (int j = 0; j < width - 2; j++)
            {
                tempCoor = new Coordinate(j, i);
                if (stageUnfolded[i, j] == 1)
                    mapTilemap.SetTile(new Vector3Int(j + 1, -i - 1, 1), rok);
                else if (stageUnfolded[i, j] == 2 && (!LobbyManager.IsOnline || LobbyManager.instance.IsHosting))
                {
                    temp = Instantiate(powerUp, tempCoor.returnAsVector(), Quaternion.identity);
                    temp.GetComponent<NetworkObject>().Spawn(true);
                }
                else if (stageUnfolded[i, j] == 3)
                {
                    k = (j < (int)(width / 2) + 1) ? 0 : 1;
                    PlayerCoordinates[(j < (int)(width / 2) + 1) ? 0 : 1, playerAmt[k]] = new Coordinate(j, i);
                    playerAmt[k]++;

                }
            }
        if (!LobbyManager.IsOnline)
            PlayerPositions[0, UnityEngine.Random.Range(0, 5)] = 1;
        else
        {
            playerAmt = new int[] { 0, 0 };
            foreach (Player p in LobbyManager.instance.CurrentLobby.Players)
            {
                if (p.Data["isLeftTeam"].Value.Equals("y"))
                    playerAmt[0]++;
                else
                    playerAmt[1]++;
            }
            string[] orderValue = LobbyManager.instance.CurrentLobby.Data["PlayerOrder"].Value.Split(",");
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 5; j++)
                {
                    if ((int)Char.GetNumericValue(orderValue[i][j]) <= playerAmt[i])
                        PlayerPositions[i, j] = (int)Char.GetNumericValue(orderValue[i][j]);
                    else
                        PlayerPositions[i, j] = 0;
                }
        }
        if ( !LobbyManager.IsOnline || LobbyManager.instance.IsHosting)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    //Spawn Bot kalau ngga ada player
                    if (PlayerPositions[i, j] == 0)
                        playerManagerReference.makeNewBot(PlayerCoordinates[i, j], i == 0);
                }
            }
        }
        Debug.Log("Fill Selesai");
        //printCoordinatesAndPositions();
    }

    public Vector3 GetPositionFromOrderID(int OrderID, bool isLeftTeam)
    {
        int eh = isLeftTeam ? 0 : 1;
        for (int i = 0; i < 5; i++)
        {
            if (PlayerPositions[eh, i] == OrderID)
                return PlayerCoordinates[eh, i].returnAsVector();
        }
        return Vector3.zero;
    }

    public void clearMap()
    {
        mapTilemap.ClearAllTiles();
    }

    void printCoordinatesAndPositions()
    {
        string result = "";
        try
        {
            for (int i = 0; i < 5; i++)
            {
                result += $"[{PlayerPositions[0, i]} : {PlayerCoordinates[0, i]}] ";
                result += $"[{PlayerPositions[1, i]} : {PlayerCoordinates[1, i]}]\n";
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        Debug.Log(result);
    }

}
