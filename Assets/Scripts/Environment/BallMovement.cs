using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallMovement : NetworkBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Vector2 direction;
    [SerializeField] bool fromPlayerTeam;
    [SerializeField] int ballScore;
    Rigidbody2D thisRigid;
    Collider2D currentCollider;
    [SerializeField] int powerupId;
    [SerializeField] GameObject thrower;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<CircleCollider2D>().enabled = false;
        thisRigid = GetComponent<Rigidbody2D>();
        if (IsServer)
        {
            GetComponent<CircleCollider2D>().enabled = true;
            InitializeClientBallClientRPC(speed, direction, fromPlayerTeam, ballScore, powerupId);
            if (currentCollider != null)
                IgnorePlayerClientRPC(currentCollider.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }

    public void initialize(float speed, Vector2 direction, bool isPlayerTeam, int ballScore, Collider2D you, GameObject thrower)
    {
        thisRigid = this.GetComponent<Rigidbody2D>();
        this.speed = speed;
        this.direction = direction;
        this.fromPlayerTeam = isPlayerTeam;
        this.ballScore = ballScore;
        this.currentCollider = you;
        if (you != null)
        Physics2D.IgnoreCollision(this.GetComponent<CircleCollider2D>(), you);
        this.thrower = thrower;
        thisRigid.rotation = - Vector2.SignedAngle(direction, Vector2.right);
    }

    public void initialize(float speed, Vector2 direction, bool isPlayerTeam, int ballScore, Collider2D you, GameObject thrower, int powerupId)
    {
        initialize(speed, direction, isPlayerTeam, ballScore, you, thrower);
        this.powerupId = powerupId;
    }
    public bool getPlayerTeam()
    {
        return fromPlayerTeam;
    }

    public int getBallPowerId()
    {
        return powerupId;
    }

    public int getBallScore()
    {
        return ballScore;
    }

    public void setBallScore(int ballScore)
    {
        this.ballScore =  ballScore;
    }

    public Vector2 getDirection()
    {
        return direction;
    }

    public void addScore(int score)
    {
        ballScore += score;
    }

    public void trySelfDestruct(GameObject collider)
    {
        if (powerupId == 0 && IsServer)
        {
            gameObject.GetComponent<NetworkObject>().Despawn(true);
            Destroy(gameObject);
        }
        else
        {
            GetComponent<BallPowerUp>().modifyBall(collider);
        }
    }

    private void FixedUpdate()
    {
        thisRigid.MovePosition((Vector2)this.transform.position + direction * Time.deltaTime * speed);
        if (powerupId != 2)
            return;
        Vector3 Rotation = new Vector3(0, 0, Time.deltaTime * speed);
        transform.Rotate(Rotation * 25);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AudioSource.PlayClipAtPoint(AudioScript.audioObject.getSound("Hit"),transform.position);
        if (collision.tag == "Wall")
        {
            if (powerupId == 5 || powerupId == 3)
                GetComponent<BallPowerUp>().modifyBall(collision.gameObject);
            else
            {
                gameObject.GetComponent<NetworkObject>().Despawn(true);
            }
        }

    }

    public void ballIsCatched(bool isPlayerTeam, int addScore, float speed, Collider2D you, GameObject thrower)
    {
        this.fromPlayerTeam = isPlayerTeam;
        this.ballScore += addScore;
        this.speed += speed;
        // Bisa kena pelempar sebelumnya
        if (currentCollider != null)
            Physics2D.IgnoreCollision(this.GetComponent<CircleCollider2D>(), currentCollider, false);
        //Ngga bisa kena pelempar baru
        Physics2D.IgnoreCollision(this.GetComponent<CircleCollider2D>(), you);
        currentCollider = you;
        this.thrower = thrower;
    }

    public void setDirection(Vector2 direction)
    {
        this.direction = direction.normalized;
    }

    public void setPowerUpID(int powerUpID)
    {
        this.powerupId = powerUpID;
    }

    public GameObject getThrower()
    {
        return thrower;
    }

    [ClientRpc]
    void IgnorePlayerClientRPC(ulong PlayerID)
    {
        foreach (SnowBrawler player in FindObjectsOfType<SnowBrawler>())
        {
            if(player.GetComponent<NetworkObject>().NetworkObjectId == PlayerID)
            {
                thrower = player.gameObject;
                GetComponent<CircleCollider2D>().enabled = true;
                Physics2D.IgnoreCollision(this.GetComponent<CircleCollider2D>(), player.GetComponent<Collider2D>());
                break;
            }
        }
    }

    [ClientRpc]
    void InitializeClientBallClientRPC(float speed, Vector2 direction, bool isPlayerTeam, int ballScore,int BallPowerID)
    {
        thisRigid = this.GetComponent<Rigidbody2D>();
        this.speed = speed;
        this.direction = direction;
        this.fromPlayerTeam = isPlayerTeam;
        this.ballScore = ballScore;
        thisRigid.rotation = -Vector2.SignedAngle(direction, Vector2.right);
        if (BallPowerID != 0)
        {
           GetComponent<SpriteRenderer>().sprite = FindObjectOfType<PowerUp>().getPowerUpSprite(BallPowerID);
            powerupId = BallPowerID;
        }
    }
}
