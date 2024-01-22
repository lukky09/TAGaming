using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class SnowBrawler : NetworkBehaviour
{
    public int ballAmount { get; set; }
    protected GameObject caughtBall { get; set; }
    public int ballPowerId { get; set; }
    public BarScoreRtc BarScoreReference { set { _barScoreRef = value; } }
    public SnowBallManager SnowballManagerRef { get { return _snowballManagerRef; } set { _snowballManagerRef = value; } }

    SnowbrawlerActionsRPC _snowBrawlerActionRPCRef;
    PowerUp _powerUpScriptRef;
    BarScoreRtc _barScoreRef;
    SnowBallManager _snowballManagerRef;
    [SerializeField] GameObject displayedBall;
    [SerializeField] GameObject numberReference;

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
    public bool canCatchBall;
    public bool isTargeted;

    Vector2 lastpos;
    public float timeDelay = 0.1f;
    float currentTimeDelay = 0;

    public void Start()
    {
        _powerUpScriptRef = FindObjectOfType<PowerUp>();
        animator = GetComponent<Animator>();
        SFXSource = GetComponent<AudioSource>();
        _snowBrawlerActionRPCRef = GetComponent<SnowbrawlerActionsRPC>();
        isAiming = false;
        canAct = true;
        runSpeed = originalRunSpeed;
        canCatchBall = true;
        isTargeted = false;
    }

    public void Update()
    {
        currentTimeDelay -= Time.deltaTime;
        //update posisi sebelumnya target untuk prediksi
        if (currentTimeDelay <= 0)
        {
            animator.SetFloat("MoveSpeed", Vector2.Distance(lastpos, transform.position));
            if ((Vector2)transform.position - (Vector2)lastpos != Vector2.zero)
                animator.SetFloat("SeeDirection", Vector2.Angle(Vector2.up, (Vector2)transform.position - (Vector2)lastpos));
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
                if (caughtBall != null && IsServer)
                    caughtBall.GetComponent<NetworkObject>().Despawn();
                caughtBall = collision.gameObject;
                GetCaughtBallServerRPC();
            }
            else
            {
                BallMovement bol = collision.gameObject.GetComponent<BallMovement>();
                if (bol.getPlayerTeam() != playerteam && _barScoreRef != null)
                    _barScoreRef.addScoreServerRPC(bol.getPlayerTeam(), bol.getBallScore());
                if (IsServer)
                    GetHitAnimationStartClientRPC(0.5f, bol.getPlayerTeam(), bol.getBallScore());
                bol.trySelfDestruct(gameObject);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void GetCaughtBallServerRPC()
    {
        caughtBall.SetActive(false);
        caughtBall.GetComponent<BallMovement>().ballIsCatched(getplayerteam(), ballScoreAdd, ballSpeedAdd, GetComponent<BoxCollider2D>(), gameObject);
        updateHoldedBallVisuals(false);
        GetCaughtBallClientRPC(caughtBall.GetComponent<NetworkObject>().NetworkObjectId);
    }

    [ClientRpc]
    void GetCaughtBallClientRPC(ulong BallID)
    {
        foreach (BallMovement Ball in FindObjectsOfType<BallMovement>())
        {
            if (Ball.GetComponent<NetworkObject>().NetworkObjectId == BallID)
            {
                caughtBall = Ball.gameObject;
                caughtBall.SetActive(false);
                caughtBall.GetComponent<BallMovement>().ballIsCatched(getplayerteam(), ballScoreAdd, ballSpeedAdd, GetComponent<BoxCollider2D>(), gameObject);
                updateHoldedBallVisuals(false);
                break;
            }
        }
    }

    public void getBall()
    {
        _snowBrawlerActionRPCRef.PickUpBallServerRPC();
    }

    public void PickUpBallFromGround()
    {
        int ballindex = _snowballManagerRef.getNearestBallIndex(transform);
        if (ballindex < 0 || Vector2.Distance(transform.position, _snowballManagerRef.getBallfromIndex(ballindex).transform.position) > ballTakeRange)
            return;
        if (_snowballManagerRef.getBallfromIndex(ballindex).GetComponent<PowerUp>())
        {
            (ballSprite, ballPowerId) = _snowballManagerRef.getBallfromIndex(_snowballManagerRef.getNearestBallIndex(transform)).GetComponent<PowerUp>().getPowerupId();
            UpdateHoldedBallsAmountAfterPickup();
        }
        else
        {
            int deletedIndex = _snowballManagerRef.getNearestBallIndex(transform, ballTakeRange);
            if (deletedIndex >= 0)
            {
                ballPowerId = 0;
                _snowballManagerRef.deleteclosestball(transform, ballTakeRange);
                UpdateHoldedBallsAmountAfterPickup();
            }
        }
    }

    public void UpdateHoldedBallsAmountAfterPickup()
    {
        if (ballPowerId == 0)
            ballSprite = ball.GetComponent<SpriteRenderer>().sprite;
        else
            ballSprite = _powerUpScriptRef.getPowerUpSprite(ballPowerId);
        ballAmount = 1;
        SFXSource.clip = AudioScript.audioObject.getSound("Get");
        SFXSource.Play();
        updateHoldedBallVisuals(false);
    }

    public void shootBall(Vector2 direction)
    {
        _snowBrawlerActionRPCRef.ThrowBallServerRPC(direction);
    }

    public void ThrowBall(Vector2 direction)
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
            ballin.GetComponent<NetworkObject>().Spawn(true);
            if (ballPowerId > 0)
                ballin.GetComponent<SpriteRenderer>().sprite = ballSprite;
            ballAmount--;
        }
        ballin.GetComponent<SpriteRenderer>().material = GetComponent<SpriteRenderer>().material;
        SFXSource.clip = AudioScript.audioObject.getSound("Yeet");
        SFXSource.Play();
        updateHoldedBallVisuals(true);
    }

    //Fungsi dipakai untuk melempar bola yang dipegang client
    public void ThrowCaughtBallClient(ulong NetworkObjectID, Vector2 ThrowDirection, Vector3 ThrowFrom)
    {
        if (caughtBall == null || NetworkObjectID != 0)
            return;
        caughtBall.transform.position = ThrowFrom;
        caughtBall.GetComponent<BallMovement>().setDirection(ThrowDirection);
        caughtBall.SetActive(true);
    }

    public void UpdateBallAmount(ulong NetworkBallID, int BallAmount, int BallPowerID)
    {
        if (NetworkBallID != 0)
            foreach (BallMovement item in FindObjectsOfType<BallMovement>())
            {
                if (item.GetComponent<NetworkObject>().NetworkObjectId == NetworkBallID)
                {
                    caughtBall = item.gameObject;
                    break;
                }
            }
        else if(NetworkBallID == 0)
            caughtBall = null;
        ballAmount = BallAmount;
        ballPowerId = BallPowerID;
    }

    public int getBallAmount()
    {
        return ballAmount + ((caughtBall == null) ? 0 : 1);
    }

    public int getOnlyGroundBallAmount()
    {
        return ballAmount;
    }

    public bool getplayerteam()
    {
        return playerteam;
    }

    public bool getIsTargeted()
    {
        return isTargeted;
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
        runSpeed = originalRunSpeed * slowPower;
        yield return new WaitForSeconds(seconds);
        GetComponent<SpriteRenderer>().color = Color.white;
        runSpeed = originalRunSpeed;
    }

    public void getHit(float seconds, GameObject snowBall)
    {
        if (!IsServer)
            return;
        BallMovement ballMovementScript = snowBall.GetComponent<BallMovement>();
        GetHitAnimationStartClientRPC(seconds, ballMovementScript.getPlayerTeam(), ballMovementScript.getBallScore());
    }


    [ClientRpc]
    void GetHitAnimationStartClientRPC(float seconds, bool BallTeam, int BallScore)
    {
        StartCoroutine(getHitNumerator(seconds, BallTeam, BallScore));
    }

    public IEnumerator getHitNumerator(float seconds, bool BallTeam, int BallScore)
    {
        if (BallTeam != playerteam && BallScore > 0)
        {
            GameObject numbers = Instantiate(numberReference);
            numbers.GetComponent<NumbersController>().setGambar(BallScore);
            numbers.GetComponent<NumbersController>().StartingPosition = transform.position;
        }
        canAct = false;
        animator.SetBool("IsHit", true);
        //Debug.Log("Kena Hit");
        yield return new WaitForSeconds(seconds);
        canAct = true;
        animator.SetBool("IsHit", false);
    }

    public void catchBall()
    {
        if (IsServer)
            CatchBallClientRPC();
        else
            CatchBallServerRPC();
    }

    [ServerRpc]
    void CatchBallServerRPC()
    {
        Debug.Log("Client Tried to catch ball");
        StartCoroutine(catchBallNumerator());
        CatchBallClientRPC();
    }

    [ClientRpc]
    void CatchBallClientRPC()
    {
        StartCoroutine(catchBallNumerator());
    }

    public IEnumerator catchBallNumerator()
    {
        runSpeed = 0;
        iscatching = true;
        canCatchBall = false;
        animator.SetBool("IsCatching", true);
        yield return new WaitForSeconds(ballCatchTimer);
        animator.SetBool("IsCatching", false);
        runSpeed = originalRunSpeed;
        iscatching = false;
        StartCoroutine(catchRecharging());
    }

    public IEnumerator catchRecharging()
    {
        GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
        yield return new WaitForSeconds(catchRecharge);
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        canCatchBall = true;
    }

    public IEnumerator shartShooting()
    {
        animator.SetBool("isShooting", true);
        canAct = false;
        runSpeed = 0;
        yield return new WaitForSeconds((0.5f * 5) / 6);
        runSpeed = originalRunSpeed;
        canAct = true;
        updateHoldedBallVisuals(true);
        animator.SetBool("isShooting", false);
    }

    public void tryCatch()
    {
        Debug.Log("Snowbrawler Catch");
        if (!iscatching)
            catchBall();
    }

    public bool getIsCatching()
    {
        return iscatching;
    }

    public Sprite getBallSprite()
    {
        return ballSprite;
    }

    void updateHoldedBallVisuals(bool isThrown)
    {
        if (GetComponent<DisplayBall>() != null && IsOwner)
            GetComponent<DisplayBall>().updateUI(isThrown);
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
