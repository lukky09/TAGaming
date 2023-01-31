using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    GameObject[] players;
    [SerializeField] Material playerMaterial;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject enemyPrefab;

    [SerializeField] GameObject levelCamera;

    private void Start()
    {
        players = new GameObject[10];
        Coordinate coor =  Coordinate.getRandomCoordinate();
        makeNewPlayer(coor);
        makeNewBot(new Coordinate(SetObjects.getWidth() - coor.xCoor, coor.yCoor),false);
        for (int i = 1; i < 5 - 4; i++)
        {
            coor = Coordinate.getRandomCoordinate();
            makeNewPlayer(coor);
            makeNewBot(new Coordinate(SetObjects.getWidth() - coor.xCoor, coor.yCoor), false);
        }
    }

    public void makeNewPlayer(Coordinate c)
    {
        Material m = new Material(playerMaterial);
        m.SetColor("_OutlineColor", Color.yellow);
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null)
            {
                players[i] = Instantiate(playerPrefab, c.returnAsVector(), Quaternion.identity);
                levelCamera.GetComponent<CameraController2D>().setCameraFollower(players[i]);
                players[i].GetComponent<SpriteRenderer>().material = new Material(m);
                break;
            }
        }
    }

    public void makeNewBot(Coordinate c, bool isPlayerTeam)
    {
        Material m = new Material(playerMaterial);
        if(isPlayerTeam)
            m.SetColor("_OutlineColor", Color.blue);
        else
        m.SetColor("_OutlineColor", Color.red);
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null)
            {
                GameObject tempEnemyPrefab = Instantiate(enemyPrefab, c.returnAsVector(), Quaternion.identity);
                tempEnemyPrefab.GetComponent<SnowBrawler>().initializeBrawler(isPlayerTeam, i);
                tempEnemyPrefab.GetComponent<SpriteRenderer>().material = new Material(m);
                players[i] = tempEnemyPrefab;
                break;
            }
        }
    }
}
