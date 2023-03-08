using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPowerUp : MonoBehaviour
{
    [SerializeField]
    int pierceScoreAdd;
    BallMovement bmRef;

    private void Start()
    {
        bmRef = GetComponent<BallMovement>();
    }

    public void modifyBall(int powerUpID)
    {
        if(powerUpID == 1)
        {
            bmRef.addScore(pierceScoreAdd);
        }
    }
}
