using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeSnowBall : MonoBehaviour
{
    public float takeRange;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        
    }

    GameObject getclosestball()
    {
        GameObject closestball = null;
        float closestrange = 999, range = 0;
        foreach (GameObject ballz in SnowBallManager.snowballs)
        {
            range = Vector2.Distance(ballz.transform.position, transform.position);
            if (range < closestrange)
            {
                closestball = ballz;
                closestrange = range;
            }
        }
        return closestball;
    }
}
