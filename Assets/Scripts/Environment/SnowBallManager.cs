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
            snowballs.Add(ballz.gameObject);
        currentrespawnTimer = respawnAmount;
    }

    public static void destroyball(int index)
    {
        GameObject ball = (GameObject)snowballs[index];
        SetObjects.setMap(-Mathf.FloorToInt(ball.transform.position.y) + 1, Mathf.FloorToInt(ball.transform.position.x) - 1, 0);
        Destroy(ball);
        snowballs.RemoveAt(index);
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
                ballz = Instantiate(snowball, new Vector3(x + 1.5f, -y - 1.5f), Quaternion.identity);
                ballz.transform.SetParent(snowballscontainer.transform, true);
                snowballs.Add(ballz);
                SetObjects.setMap(y,x,4);
            }
        }
    }

    static public bool getDeleteclosestball(Transform objecttransform, float rangetreshold, bool delete)
    {
        bool isdeleted = true;
        float closestrange = 999, range = 0;
        int i = 0, index = -1;
        foreach (GameObject ballz in snowballs)
        {
            range = Vector2.Distance(ballz.transform.position, objecttransform.position);
            if (range < closestrange)
            {
                closestrange = range;
                index = i;
            }
            i++;
        }
        if (delete && closestrange < rangetreshold  && index >= 0)
        {
            destroyball(index);
            isdeleted = true;
        }
        else if (!delete && closestrange < rangetreshold)
            isdeleted = true;
        return isdeleted;
    }
}
