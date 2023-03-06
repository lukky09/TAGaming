using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeSnowBall : MonoBehaviour
{
    public float takeRange;

    int ballPowerId;
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
            if (SnowBallManager.getBallfromIndex(SnowBallManager.getNearestBallIndex(transform)).GetComponent<PowerUp>())
            {
                ballPowerId = SnowBallManager.getBallfromIndex(SnowBallManager.getNearestBallIndex(transform)).GetComponent<PowerUp>().getPowerupId();
            }
            else
            {
                ballPowerId = 0;
                bool deleted = SnowBallManager.getDeleteclosestball(transform, takeRange, true);
                if (deleted)
                    currentballamount = 3;
            }
        }
    }

    public int getPowerId()
    {
        return ballPowerId;
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
