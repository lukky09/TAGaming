using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBrawler : MonoBehaviour
{

    protected int ballAmount { get; set; }
    protected float currentBallCatchTimer { get; set; }
    protected GameObject caughtBall { get; set; }
    protected int ballPowerId { get; set; }

    public bool playerteam;
    public float throwSpeed;
    public float runSpeed;
    public int ballScoreInitial;
    public int ballScoreAdd;
    public float ballSpeedAdd;
    public float ballCatchTimer;
    public float ballTakeRange;
    public GameObject ball;


    public void initializeBrawler(bool playerteam, float throwSpeed,float runSpeed,int ballScoreAdd, float ballSpeedAdd,float ballCatchTimer,float ballTakeRange)
    {
        this.ballTakeRange = ballTakeRange;
        this.runSpeed = runSpeed;
        initializeBrawler(playerteam, throwSpeed, ballScoreAdd, ballSpeedAdd, ballCatchTimer);
    }

    public void initializeBrawler(bool playerteam, float throwSpeed, int ballScoreAdd, float ballSpeedAdd, float ballCatchTimer)
    {
        this.playerteam = playerteam;
        ballAmount = 0;
        this.throwSpeed = throwSpeed;
        this.ballCatchTimer = ballCatchTimer;
        this.ballScoreAdd = ballScoreAdd;
        this.ballSpeedAdd = ballSpeedAdd;
        currentBallCatchTimer = 0;
    }

    public void Update()
    {
        currentBallCatchTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            if (currentBallCatchTimer > 0)
            {
                if (caughtBall != null)
                    Destroy(caughtBall);
                caughtBall = collision.gameObject;
                caughtBall.SetActive(false);
                caughtBall.GetComponent<BallMovement>().ballIsCatched(getplayerteam(), ballScoreAdd, ballSpeedAdd, GetComponent<BoxCollider2D>());
            }
            else
            {
                BallMovement bol = collision.gameObject.GetComponent<BallMovement>();
                if (bol.getPlayerTeam() != playerteam)
                    BarScoreManager.addscore(bol.getPlayerTeam(), bol.getBallScore());
                bol.trySelfDestruct();
            }
        }
    }

    public void getBall()
    {
        if (SnowBallManager.getBallfromIndex(SnowBallManager.getNearestBallIndex(transform)).GetComponent<PowerUp>())
        {
            ballPowerId = SnowBallManager.getBallfromIndex(SnowBallManager.getNearestBallIndex(transform)).GetComponent<PowerUp>().getPowerupId();
            if (ballPowerId > 0)
                ballAmount = 1;
        }
        else
        {
            int deletedIndex = SnowBallManager.getNearestBallIndex(transform, ballTakeRange);
            if (deletedIndex >= 0)
            {
                ballPowerId = 0;
                SnowBallManager.deleteclosestball(transform, ballTakeRange);
                ballAmount = 3;
            }
        }
    }

    public void shootBall(Vector2 direction)
    {
        GameObject ballin;
        if (caughtBall != null)
        {
            ballin = caughtBall;
            ballin.GetComponent<BallMovement>().setDirection(direction);
            ballin.transform.position = this.transform.position;
            ballin.SetActive(true);
            caughtBall = null;
        }
        else
        {
            Debug.Log(ballAmount);
            ballin = Instantiate(ball, (Vector2)this.transform.position, Quaternion.identity);
            ballin.GetComponent<BallMovement>().initialize(throwSpeed, direction, playerteam, ballScoreInitial, this.GetComponent<BoxCollider2D>(), ballPowerId);
            ballAmount--;
            Debug.Log(ballAmount);
        }
    }

    public void catchBall()
    {
        currentBallCatchTimer = ballCatchTimer;
    }

    public int getBallAmount()
    {
        return ballAmount;
    }

    public bool getplayerteam()
    {
        return playerteam;
    }

    public float getCatchTimer()
    {
        return currentBallCatchTimer;
    }

    public GameObject getCaughtBall()
    {
        return caughtBall;
    }
}
