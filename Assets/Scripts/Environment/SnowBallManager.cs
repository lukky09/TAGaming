using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBallManager : MonoBehaviour
{
    public static List<GameObject> snowballs;
    public GameObject snowballscontainer;

    [SerializeField] GameObject snowball;
    [SerializeField] float respawnTime;
    [SerializeField] int respawnAmount;
    float currentrespawnTimer;

    // Start is called before the first frame update
    void Start()
    {
        snowballs = new List<GameObject>();
        foreach (Transform ballz in snowballscontainer.transform)
        {
            snowballs.Add(ballz.gameObject);
        }
        currentrespawnTimer = respawnTime;
    }

    public static void destroyball(int index)
    {
        GameObject ball = snowballs[index];
        Coordinate ballcoor = AStarAlgorithm.vectorToCoordinate(ball.transform.position);
        SetObjects.setMap(ballcoor.yCoor, ballcoor.xCoor, 0);
        Destroy(ball);
        snowballs.RemoveAt(index);
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
            x = Mathf.RoundToInt(Random.Range(0, SetObjects.getWidth() - 2));
            y = Mathf.RoundToInt(Random.Range(0, SetObjects.getHeight() - 2));
            if (SetObjects.getMap(false)[y, x] == 0)
            {
                ballz = Instantiate(snowball, new Vector3(x + 1.5f, -y - 0.5f), Quaternion.identity);
                ballz.transform.SetParent(snowballscontainer.transform, true);
                snowballs.Add(ballz);
                //Debug.Log("Bola ke-" + i + " = " + x + " " + y);
                SetObjects.setMap(y, x, 4);
            }
        }
    }

    static public bool deleteclosestball(Transform objecttransform, float rangetreshold)
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

    static public GameObject getClosestBall(Transform objecttransform, float rangetreshold)
    {
        int index = getNearestBallIndex(objecttransform, rangetreshold);
        return snowballs[index];
    }

    public static int getNearestBallIndex(Transform objectTracked)
    {
        float closestrange = 999,range;
        int i = 0, index = -1;
        foreach (GameObject ballz in snowballs)
        {
            range = Vector2.Distance(ballz.transform.position, objectTracked.position);
            if (range < closestrange && (ballz.GetComponent<PowerUp>() == null || ballz.GetComponent<PowerUp>().isActive()))
            {
                closestrange = range;
                index = i;
            }
            i++;
        }
        return index;
    }

    public static int getNearestBallIndex(Transform objectTracked, float range)
    {
        float closestrange = 999, currrange;
        int i = 0, index = -1;
        foreach (GameObject ballz in snowballs)
        {
            currrange = Vector2.Distance(ballz.transform.position, objectTracked.position);
            if (currrange < range && currrange<closestrange && (ballz.GetComponent<PowerUp>() == null || ballz.GetComponent<PowerUp>().isActive()))
            {
                closestrange = currrange;
                index = i;
            }
            i++;
        }
        if (index > -1 && Vector2.Distance((snowballs[index]).transform.position, objectTracked.position) < range)
            return index;
        else
            return -1;
    }

    public static GameObject getBallfromIndex(int index)
    {
        if (snowballs.Count > 0)
            return snowballs[index];
        return null;
    }

    public static int getBallAmount()
    {
        return snowballs.Count;
    }
}
