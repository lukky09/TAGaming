using Scriban.Syntax;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

public class PlayersManager : MonoBehaviour
{
    GameObject[] players;
    [SerializeField] Material material;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] bool isAIActive;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] bool spawnPlayer;
    [SerializeField] GameObject playersContainer;

    [SerializeField] GameObject levelCamera;

    List<bool[,]> accesibleAreas;
    List<int[]> areaCorners;

    private void Start()
    {
        players = new GameObject[10];
        foreach (Transform item in playersContainer.transform)
        {
            players[getFirstNullPlayerIndex()] = item.gameObject;
        }
        if (SetObjects.getMap(false) != null)
        {
            Queue<Coordinate> q = new Queue<Coordinate>();
            Coordinate c, tempCoor;
            accesibleAreas = new List<bool[,]>();
            areaCorners = new List<int[]>();
            int[,] currmap = SetObjects.getMap(false);
            int[] coors;
            bool[,] isChecked = new bool[currmap.GetLength(0), currmap.GetLength(1)], map;
            for (int i = 0; i < currmap.GetLength(0); i++)
            {
                for (int j = 0; j < currmap.GetLength(1); j++)
                {
                    if (!isChecked[i, j] && currmap[i, j] != 1)
                    {
                        coors = new int[4] { j, i, -1, -1 }; // x1,y1 (kiri atas),x2,y2 (kanan bawah)
                        map = new bool[currmap.GetLength(0), currmap.GetLength(1)];
                        isChecked[i, j] = true;
                        map[i, j] = true;
                        q.Enqueue(new Coordinate(j, i));
                        while (q.Count > 0)
                        {
                            c = q.Dequeue();
                            if (c.xCoor < coors[0]) coors[0] = c.xCoor;
                            else if (c.xCoor >= coors[2]) coors[2] = c.xCoor;
                            if (c.yCoor < coors[1]) coors[1] = c.yCoor;
                            else if (c.yCoor >= coors[3]) coors[3] = c.yCoor;
                            for (int k = 0; k < 4; k++)
                            {
                                tempCoor = new Coordinate(c.xCoor + Mathf.RoundToInt(Mathf.Sin(k * Mathf.PI / 2)), c.yCoor + Mathf.RoundToInt(Mathf.Cos(k * Mathf.PI / 2)));
                                if (tempCoor.xCoor >= 0 && tempCoor.yCoor >= 0 && tempCoor.yCoor < SetObjects.getHeight() && tempCoor.xCoor < currmap.GetLength(1) && currmap[tempCoor.yCoor, tempCoor.xCoor] != 1 && !isChecked[tempCoor.yCoor, tempCoor.xCoor])
                                {
                                    isChecked[tempCoor.yCoor, tempCoor.xCoor] = true;
                                    map[tempCoor.yCoor, tempCoor.xCoor] = true;
                                    q.Enqueue(tempCoor);
                                }
                            }
                        }
                        accesibleAreas.Add(map);
                        areaCorners.Add(coors);
                    }
                }
            }
            Debug.Log(accesibleAreas.Count);
        }

    }

    public void makeNewPlayer(Coordinate c)
    {
        int i = getFirstNullPlayerIndex();
        players[i] = Instantiate(playerPrefab, c.returnAsVector(), Quaternion.identity);
        levelCamera.GetComponent<CameraController2D>().setCameraFollower(players[i], false);
        players[i].transform.SetParent(playersContainer.transform);
    }

    public void makeNewBot(Coordinate c, bool isPlayerTeam)
    {
        int i = getFirstNullPlayerIndex();
        GameObject tempEnemyPrefab = Instantiate(enemyPrefab, c.returnAsVector(), Quaternion.identity);
        if (isPlayerTeam)
        {
            tempEnemyPrefab.GetComponent<SnowBrawler>().playerteam = true;
            tempEnemyPrefab.GetComponent<ColorTaker>().id = 0;
        }
        for (int j = 0; j < accesibleAreas.Count; j++)
        {
            if (accesibleAreas[j][c.yCoor, c.xCoor])
            {
                tempEnemyPrefab.GetComponent<BotActions>().setMapSegmentID(j + 1, this);
                break;
            }
        }
        if (!isAIActive)
            tempEnemyPrefab.GetComponent<StateMachine>().enabled = false;
        players[i] = tempEnemyPrefab;
        players[i].transform.SetParent(playersContainer.transform);
    }

    public GameObject getnearestPlayer(Transform player, bool includeCollision, float visionRange)
    {
        float closestrange = 999, range;
        int i = 0, index = -1;
        RaycastHit2D cekKolisi;
        bool isEnemy;
        foreach (GameObject currplayer in players)
        {
            if (currplayer != null)
            {
                isEnemy = currplayer.GetComponent<SnowBrawler>().getplayerteam() != player.GetComponent<SnowBrawler>().getplayerteam();
                Vector2 direction = Vector3.Normalize(currplayer.transform.position - player.transform.position);
                cekKolisi = Physics2D.CircleCast(player.position, 0.4f, direction, visionRange, 64);
                //Physics2D.CircleCast(currentPoint.returnAsVector(), circleSize, arah, dist, 64);
                range = Vector2.Distance(player.transform.position, currplayer.transform.position);
                if (range < closestrange && range > 0 && !(includeCollision && cekKolisi) && isEnemy)
                {
                    closestrange = range;
                    index = i;
                }
                i++;
            }
        }
        if (index >= 0)
            return players[index];
        else
            return null;
    }

    public Coordinate getRandomSpot(int mapIndex)
    {
        bool[,] currAccesibleAreaa = accesibleAreas[mapIndex];
        int[] currAreaCorners = areaCorners[mapIndex];
        Coordinate tempCoor;
        do
        {
            tempCoor = new Coordinate(Random.Range(currAreaCorners[0], currAreaCorners[2] + 1), Random.Range(currAreaCorners[1], currAreaCorners[3] + 1));
        } while (!currAccesibleAreaa[tempCoor.yCoor, tempCoor.xCoor]);
        return tempCoor;
    }

    int getFirstNullPlayerIndex()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null)
                return i;
        }
        return -1;
    }

    public void activatePlayersScript(bool activate)
    {
        MonoBehaviour[] scripts;
        foreach (Transform item in playersContainer.transform)
        {
            scripts = item.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                script.enabled = activate;
            }
            item.GetComponent<ColorTaker>().enabled = true;
            if (item.GetComponent<CoordinateMovement>() != null)
                item.GetComponent<CoordinateMovement>().enabled = true;
        }
    }

}
