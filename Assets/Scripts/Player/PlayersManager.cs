using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayersManager : MonoBehaviour
{
    GameObject[] players;
    [SerializeField] Material material;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Color playerColor;
    [SerializeField] bool isAIActive;
    [SerializeField] Color teamColor;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Color enemyColor;
    [SerializeField] bool spawnPlayer;
    [SerializeField] GameObject playersContainer;

    [SerializeField] GameObject levelCamera;

    private void Start()
    {
        players = new GameObject[10];
        foreach (Transform item in playersContainer.transform)
        {
            players[getFirstNullPlayerIndex()] = item.gameObject;
        }
    }

    public void makeNewPlayer(Coordinate c)
    {
        Material m = new Material(material);
        m.SetColor("_OutlineColor", playerColor);
        int i = getFirstNullPlayerIndex();
        players[i] = Instantiate(playerPrefab, c.returnAsVector(), Quaternion.identity);
        levelCamera.GetComponent<CameraController2D>().setCameraFollower(players[i], false);
        players[i].GetComponent<SpriteRenderer>().material = new Material(m);
        players[i].transform.SetParent(playersContainer.transform);
    }

    public void makeNewBot(Coordinate c, bool isPlayerTeam)
    {
        Material m = new Material(material);
        if (isPlayerTeam)
            m.SetColor("_OutlineColor", teamColor);
        else
            m.SetColor("_OutlineColor", enemyColor);
        int i = getFirstNullPlayerIndex();
        GameObject tempEnemyPrefab = Instantiate(enemyPrefab, c.returnAsVector(), Quaternion.identity);
        tempEnemyPrefab.GetComponent<SnowBrawler>().initializeBrawler(isPlayerTeam, 7.5f, 15, 2, 1, 1, 0.5f);
        tempEnemyPrefab.GetComponent<SpriteRenderer>().material = new Material(m);
        if (!isAIActive)
            tempEnemyPrefab.GetComponent<StateMachine>().enabled = false;
        players[i] = tempEnemyPrefab;
        players[i].transform.SetParent(playersContainer.transform);
    }

    public GameObject getnearestPlayer(Transform player,bool includeCollision)
    {
        float closestrange = 999, range;
        int i = 0, index = -1;
        RaycastHit2D cekKolisi;
        foreach (GameObject currplayer in players)
        {
            if (currplayer != null)
            {
                cekKolisi = Physics2D.Linecast(player.position, currplayer.transform.position, 64);
                range = Vector2.Distance(player.transform.position, currplayer.transform.position);
                if (range < closestrange && range > 0 && !(includeCollision && cekKolisi))
                {
                    closestrange = range;
                    index = i;
                }
                i++;
            }
        }
        if (index > 0)
            return players[index];
        else
            return null;
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
}
