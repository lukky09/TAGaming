using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBrawler : MonoBehaviour
{
    protected bool playerteam { get; set; }
    protected int id { get; set; }

    public void initializeBrawler(bool playerteam, int id)
    {
        this.playerteam = playerteam;
        this.id = id;
    }

    public int getid()
    {
        return id;
    }

    public bool getplayerteam()
    {
        return playerteam;
    }
}
