using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBrawler : MonoBehaviour
{
    protected int ballAmount { get; set; }
    protected GameObject caughtBall { get; set; }
    protected int ballPowerId { get; set; }

    public bool playerteam;
    public float throwSpeed;
    public float originalRunSpeed;
    public float runSpeed;
    public int ballScoreInitial;
    public int ballScoreAdd;
    public float ballSpeedAdd;
    public float ballCatchTimer;
    public float ballTakeRange;
    public GameObject ball;
    public bool isAiming;
    bool iscatching;
    Sprite ballSprite;
    Animator animator;
    bool canMove;

    Vector2 lastpos;
    public float timeDelay = 0.1f;
    float currentTimeDelay = 0;

    public void Start()
    {
        animator = GetComponent<Animator>();
        isAiming = false;
        canMove = true;
        runSpeed = originalRunSpeed;
    }

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
    }

    public void Update()
    {
        currentTimeDelay -= Time.deltaTime;
        //update posisi sebelumnya target untuk prediksi
        if (currentTimeDelay <= 0)
        {
            animator.SetFloat("MoveSpeed", Vector2.Distance(lastpos,transform.position));
            lastpos = transform.position;
            currentTimeDelay = timeDelay;
        }
        animator.SetBool("IsAiming", isAiming);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            if (iscatching)
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
                bol.trySelfDestruct(gameObject);
            }
        }
    }

    public void getBall()
    {
        int ballindex = SnowBallManager.getNearestBallIndex(transform);
        if (ballindex>= 0 && SnowBallManager.getBallfromIndex(ballindex).GetComponent<PowerUp>())
        {
            (ballSprite,ballPowerId) = SnowBallManager.getBallfromIndex(SnowBallManager.getNearestBallIndex(transform)).GetComponent<PowerUp>().getPowerupId();
            if (ballPowerId > 0)
                ballAmount = 1;
        }
        else if (ballindex >= 0)
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
            ballin = Instantiate(ball, (Vector2)this.transform.position, Quaternion.identity);
            ballin.GetComponent<BallMovement>().initialize(throwSpeed, direction, playerteam, ballScoreInitial, this.GetComponent<BoxCollider2D>(), gameObject, ballPowerId);
            ballAmount--;
            if (ballPowerId > 0)
                ballin.GetComponent<SpriteRenderer>().sprite = ballSprite;
        }
        ballin.GetComponent<SpriteRenderer>().material = GetComponent<SpriteRenderer>().material;
    }

    public int getBallAmount()
    {
        return ballAmount + ((caughtBall == null) ? 0 : 1);
    }

    public bool getplayerteam()
    {
        return playerteam;
    }

    public GameObject getCaughtBall()
    {
        return caughtBall;
    }

    public void slowDown(float movementSpeedSlow, float slowTime)
    {
        StartCoroutine(slowDownNumerator(movementSpeedSlow, slowTime));
    }

    IEnumerator slowDownNumerator(float slowPower,float seconds)
    {
        runSpeed = originalRunSpeed * slowPower;
        yield return new WaitForSeconds(seconds);
        runSpeed = originalRunSpeed;
    }
    IEnumerator catchBall()
    {
        runSpeed = 0;
        iscatching = true;
        animator.SetBool("IsCatching", true);
        yield return new WaitForSeconds(ballCatchTimer);
        animator.SetBool("IsCatching", false);
        runSpeed = originalRunSpeed;
        iscatching = false;
    }

    public void shartShooting()
    {
        animator.SetBool("isShooting",true);
        runSpeed = 0;
    }

    public void stopShooting()
    {
        runSpeed = originalRunSpeed;
        animator.SetBool("isShooting", false);
    }

    public void tryCatch()
    {
        if (!iscatching)
            StartCoroutine(catchBall());
    }
}
