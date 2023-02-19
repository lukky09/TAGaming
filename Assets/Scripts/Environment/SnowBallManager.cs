using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBallManager : MonoBehaviour
{
    public static ArrayList snowballs;
    public GameObject snowballscontainer;

    [SerializeField] GameObject snowball;
    [SerializeField] float respawnTime;
    [SerializeField] int respawnAmount;
    float currentrespawnTimer;

    // Start is called before the first frame update
    void Start()
    {
        snowballs = new ArrayList();
        foreach (Transform ballz in snowballscontainer.GetComponentsInChildren<Transform>())
        {
            snowballs.Add(ballz.gameObject);
        }
        //Ini perlu karena parentnya termasuk dalam getComponentInChildren entah kenapa
        snowballs.RemoveAt(0);
        currentrespawnTimer = respawnTime;
    }

    public static void destroyball(int index)
    {
        GameObject ball = (GameObject)snowballs[index];
        Coordinate ballcoor = AStarAlgorithm.vectorToCoordinate(ball.transform.position);
        SetObjects.setMap(ballcoor.yCoor, ballcoor.xCoor, 0);
        Destroy(ball);
        snowballs.RemoveAt(index);
        snowballs.TrimToSize();
    }

    public void Update()
    {
        if(currentrespawnTimer <= 0)
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
            x = Mathf.RoundToInt(Random.Range(0,SetObjects.getWidth()-2));
            y = Mathf.RoundToInt(Random.Range(0, SetObjects.getHeight()-2));
            if (SetObjects.getMap(false)[y,x] == 0)
            {
                ballz = Instantiate(snowball, new Vector3(x + 1.5f, -y - 0.5f), Quaternion.identity);
                ballz.transform.SetParent(snowballscontainer.transform, true);
                snowballs.Add(ballz);
                //Debug.Log("Bola ke-" + i + " = " + x + " " + y);
                SetObjects.setMap(y,x,4);
            }
        }
    }

    static public bool getDeleteclosestball(Transform objecttransform, float rangetreshold, bool delete)
    {
        bool isdeleted = false;
        int index = getNearestBallIndex(objecttransform, rangetreshold);
        if (index >= 0)
        {
            if (delete)
                destroyball(index);
            isdeleted = true;
        }
        return isdeleted;
    }

    public static int getNearestBallIndex(Transform objectTracked)
    {
        float closestrange = 999, range = 0;
        int i = 0, index = -1;
        foreach (GameObject ballz in snowballs)
        {
            range = Vector2.Distance(ballz.transform.position, objectTracked.position);
            if (range < closestrange)
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
        int index = getNearestBallIndex(objectTracked);
        if (Vector2.Distance(((GameObject)snowballs[index]).transform.position, objectTracked.position) < range)
            return index;
        else
            return -1;
    }

    public static GameObject getBallfromIndex(int index)
    {
        return (GameObject)snowballs[index];
    }
}
