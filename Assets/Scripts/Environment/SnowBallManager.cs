using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnowBallManager : MonoBehaviour
{
    [SerializeField] GameObject _snowballsContainerPrefab;
    GameObject _snowballsContainer;
    bool _isCurrentlyOnline;

    [SerializeField] GameObject snowball;
    [SerializeField] float respawnTime;
    [SerializeField] int respawnAmount;
    float currentrespawnTimer;


    // Start is called before the first frame update
    void Start()
    {
        _isCurrentlyOnline = LobbyManager.IsOnline;
        currentrespawnTimer = respawnTime;
        //Kalau Tutorial
        if (SceneManager.GetActiveScene().name.Equals("Tutorial"))
        {
            _snowballsContainer = gameObject;
            foreach (SnowballSpawner spawner in FindObjectsOfType<SnowballSpawner>())
            {
                spawner.snowballContainer = gameObject;
            }
        }
        else if (!_isCurrentlyOnline || LobbyManager.instance.IsHosting)
        {
            _snowballsContainer = Instantiate(_snowballsContainerPrefab);
            _snowballsContainer.transform.position = Vector3.zero;
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.isActiveAndEnabled)
                _snowballsContainer.GetComponent<NetworkObject>().Spawn(true);
            FindObjectOfType<SetObjects>().powerUpContainer = _snowballsContainer;
            foreach (PowerUp powerUp in FindObjectsOfType<PowerUp>())
                powerUp.transform.SetParent(_snowballsContainer.transform);
            foreach (SnowBrawler brawler in FindObjectsOfType<SnowBrawler>())
                brawler.SnowballManagerRef = this;

        }
    }

    public void destroyball(int index)
    {
        GameObject ball = _snowballsContainer.transform.GetChild(index).gameObject;
        Coordinate ballcoor = AStarAlgorithm.vectorToCoordinate(ball.transform.position);
        if (SetObjects.getMap(true) != null)
            SetObjects.setMap(ballcoor.yCoor, ballcoor.xCoor, 0);
        Destroy(ball);
    }

    public void Update()
    {
        if (currentrespawnTimer <= 0)
        {
            currentrespawnTimer = respawnTime;
            putballs();
        }
        currentrespawnTimer -= Time.deltaTime;
    }


    void putballs()
    {
        int x, y;
        GameObject ballz;
        for (int i = 0; i < respawnAmount; i++)
        {
            x = Mathf.RoundToInt(UnityEngine.Random.Range(0, SetObjects.getWidth() - 2));
            y = Mathf.RoundToInt(UnityEngine.Random.Range(0, SetObjects.getHeight() - 2));
            if (SetObjects.getMap(false)[y, x] == 0 && (!LobbyManager.IsOnline ||LobbyManager.instance.IsHosting))
            {
                ballz = Instantiate(snowball, new Vector3(x + 1.5f, -y - 0.5f), Quaternion.identity);
                ballz.GetComponent<NetworkObject>().Spawn(true);
                ballz.transform.SetParent(_snowballsContainer.transform, true);
                //Debug.Log("Bola ke-" + i + " = " + x + " " + y);
                SetObjects.setMap(y, x, 4);
            }
        }
    }

    public void addBallinVector(Vector2 v)
    {
        GameObject ballz;
        ballz = Instantiate(snowball);
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.isActiveAndEnabled)
            ballz.transform.SetParent(_snowballsContainer.transform, true);
        ballz.transform.position = v;

    }

    public bool deleteclosestball(Transform objecttransform, float rangetreshold)
    {
        bool isdeleted = false;
        int index = getNearestBallIndex(objecttransform, rangetreshold);
        if (index >= 0)
        {
            destroyball(index);
            isdeleted = true;
        }
        return isdeleted;
    }

    public GameObject getClosestBall(Transform objecttransform, float rangetreshold)
    {
        int index = getNearestBallIndex(objecttransform, rangetreshold);
        return _snowballsContainer.transform.GetChild(index).gameObject;
    }

    public int getNearestBallIndex(Transform objectTracked)
    {
        float closestrange = 999, range;
        int i = 0, index = -1;
        foreach (Transform ballz in _snowballsContainer.transform)
        {
            range = Vector2.Distance(ballz.position, objectTracked.position);
            if (range < closestrange && (ballz.GetComponent<PowerUp>() == null || ballz.GetComponent<PowerUp>().isActive()))
            {
                closestrange = range;
                index = i;
            }
            i++;
        }
        return index;
    }

    public int getNearestBallIndex(Transform objectTracked, float range)
    {
        float closestrange = 999, currrange;
        int i = 0, index = -1;
        foreach (Transform ballz in _snowballsContainer.transform)
        {
            currrange = Vector2.Distance(ballz.position, objectTracked.position);
            if (currrange < range && currrange < closestrange && (ballz.GetComponent<PowerUp>() == null || ballz.GetComponent<PowerUp>().isActive()))
            {
                closestrange = currrange;
                index = i;
            }
            i++;
        }
        if (index > -1 && Vector2.Distance(_snowballsContainer.transform.GetChild(index).gameObject.transform.position, objectTracked.position) < range)
            return index;
        else
            return -1;
    }

    public int getIndexfromSnowball(GameObject go)
    {
        int i = 0;
        foreach (Transform item in _snowballsContainer.transform)
        {
            if (item.gameObject == go)
                return i;
            i++;
        }
        return -1;
    }

    public GameObject getBallfromIndex(int index)
    {

        try
        {
            if (_snowballsContainer.transform.childCount > 0)
                return _snowballsContainer.transform.GetChild(index).gameObject;
            return null;
        }
        catch (System.Exception)
        {
            return null;
        }

    }

    public bool isAnyBallNear(Vector2 position)
    {
        foreach (Transform item in _snowballsContainer.transform)
        {
            if (Vector2.Distance(position, item.position) < 1 && (item.GetComponent<PowerUp>() == null || item.GetComponent<PowerUp>().isActive()))
                return true;
        }
        return false;

    }

    public int getBallAmount()
    {
        return _snowballsContainer.transform.childCount;
    }
}
