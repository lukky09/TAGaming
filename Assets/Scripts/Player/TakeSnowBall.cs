using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeSnowBall : MonoBehaviour
{
    public float takeRange;

    int currentballamount;
    GameObject closestball;
    int ballindex;

    // Start is called before the first frame update
    private void Start()
    {
        currentballamount = 0;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Vector2.Distance(closestball.transform.position, transform.position) < takeRange)
            {
                SnowBallManager.destroyball(ballindex);
                currentballamount = 3;
            }
        }
    }

    private void FixedUpdate()
    {
        (closestball,ballindex) = getclosestball();
    }

    (GameObject,int) getclosestball()
    {
        GameObject closestball = null;
        float closestrange = 999, range = 0;
        int i = 0,index = -1;
        foreach (GameObject ballz in SnowBallManager.snowballs)
        {
            range = Vector2.Distance(ballz.transform.position, transform.position);
            if (range < closestrange)
            {
                closestball = ballz;
                closestrange = range;
                index = i;
            }
            i++;
        }
        return (closestball, index);
    }

    public int getballamount()
    {
        return currentballamount;
    }

    public void decreaseballamount()
    {
        this.currentballamount--;
    }
}
