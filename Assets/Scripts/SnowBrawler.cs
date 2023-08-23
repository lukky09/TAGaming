using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SnowBrawler : MonoBehaviour
{
    protected int ballAmount { get; set; }
    protected GameObject caughtBall { get; set; }
    protected int ballPowerId { get; set; }

    [SerializeField] GameObject displayedBall;
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
    public float catchRecharge;
    bool iscatching;
    Sprite ballSprite;
    Animator animator;
    AudioSource SFXSource;
    public bool canAct;
    bool canCatchBall;

    Vector2 lastpos;
    public float timeDelay = 0.1f;
    float currentTimeDelay = 0;

    public void Start()
    {
        animator = GetComponent<Animator>();
        SFXSource = GetComponent<AudioSource>();
        isAiming = false;
        canAct = true;
        runSpeed = originalRunSpeed;
        canCatchBall = true;
    }

    public void Update()
    {
        currentTimeDelay -= Time.deltaTime;
        //update posisi sebelumnya target untuk prediksi
        if (currentTimeDelay <= 0)
        {
            animator.SetFloat("MoveSpeed", Vector2.Distance(lastpos, transform.position));
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
                updateHoldedBallVisuals();
            }
            else
            {
                BallMovement bol = collision.gameObject.GetComponent<BallMovement>();
                if (bol.getPlayerTeam() != playerteam)
                    BarScoreManager.addscore(bol.getPlayerTeam(), bol.getBallScore());
                bol.trySelfDestruct(gameObject);
                StartCoroutine(getHitNumerator(0.5f));
            }
        }
    }

    public void getBall()
    {
        int ballindex = SnowBallManager.Instance.getNearestBallIndex(transform);
        Debug.Log(ballindex);
        if (ballindex < 0 || Vector2.Distance(transform.position, SnowBallManager.Instance.getBallfromIndex(ballindex).transform.position) > ballTakeRange)
            return;
        if (SnowBallManager.Instance.getBallfromIndex(ballindex).GetComponent<PowerUp>())
        {
            (ballSprite, ballPowerId) = SnowBallManager.Instance.getBallfromIndex(SnowBallManager.Instance.getNearestBallIndex(transform)).GetComponent<PowerUp>().getPowerupId();
            if (ballPowerId > 0)
            {
                displayedBall.GetComponent<SpriteRenderer>().sprite = ballSprite;
                ballAmount = 1;
            }
        }
        else
        {
            int deletedIndex = SnowBallManager.Instance.getNearestBallIndex(transform, ballTakeRange);
            if (deletedIndex >= 0)
            {
                ballPowerId = 0;
                SnowBallManager.Instance.deleteclosestball(transform, ballTakeRange);
                ballAmount = 3;
                ballSprite = ball.GetComponent<SpriteRenderer>().sprite;
            }
        }
        SFXSource.clip = AudioScript.audioObject.getSound("Get");
        SFXSource.Play();
        updateHoldedBallVisuals();
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
        SFXSource.clip = AudioScript.audioObject.getSound("Yeet");
        SFXSource.Play();
        ballin.GetComponent<SpriteRenderer>().material = GetComponent<SpriteRenderer>().material;
        updateHoldedBallVisuals();
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

    IEnumerator slowDownNumerator(float slowPower, float seconds)
    {
        GetComponent<SpriteRenderer>().color = new Color(11 / 255, 211 / 255, 1);
        Debug.Log(slowPower);
        runSpeed = originalRunSpeed * slowPower;
        yield return new WaitForSeconds(seconds);
        GetComponent<SpriteRenderer>().color = Color.white;
        runSpeed = originalRunSpeed;
    }

    IEnumerator getHitNumerator(float seconds)
    {
        canAct = false;
        animator.SetBool("IsHit", true);
        yield return new WaitForSeconds(seconds);
        canAct = true;
        animator.SetBool("IsHit", false);
    }

    public IEnumerator catchBall()
    {
        runSpeed = 0;
        iscatching = true;
        animator.SetBool("IsCatching", true);
        yield return new WaitForSeconds(ballCatchTimer);
        animator.SetBool("IsCatching", false);
        runSpeed = originalRunSpeed;
        iscatching = false;
        StartCoroutine(catchRecharging());
    }

    public IEnumerator catchRecharging()
    {
        canCatchBall = false;
        GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
        yield return new WaitForSeconds(catchRecharge);
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        canCatchBall = true;
    }

    public void shartShooting()
    {
        animator.SetBool("isShooting", true);
        canAct = false;
        runSpeed = 0;
    }

    public void stopShooting()
    {
        runSpeed = originalRunSpeed;
        canAct = true;
        updateHoldedBallVisuals();
        animator.SetBool("isShooting", false);
    }

    public void tryCatch()
    {
        if (!iscatching)
            StartCoroutine(catchBall());
    }

    void updateHoldedBallVisuals()
    {
        if (caughtBall == null && ballAmount == 0)
        {
            displayedBall.SetActive(false);
            return;
        }
        GetComponent<Animator>().enabled = false;
        displayedBall.SetActive(true);
        GetComponent<Animator>().enabled = true;
        if (caughtBall != null)
        {
            displayedBall.GetComponent<SpriteRenderer>().sprite = caughtBall.GetComponent<SpriteRenderer>().sprite;
            return;
        }
        displayedBall.GetComponent<SpriteRenderer>().sprite = ballSprite;
    }
}
