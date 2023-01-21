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
            Debug.Log(takeRange);
            bool deleted = SnowBallManager.getDeleteclosestball(transform, takeRange,true);
            if (deleted)
                currentballamount = 3;
        }
    }

    private void FixedUpdate()
    {
        
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
