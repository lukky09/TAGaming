using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallMovement : MonoBehaviour
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
        thisRigid = this.GetComponent<Rigidbody2D>();
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
        if (powerupId == 0)
            Destroy(gameObject);
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
        AudioSource.PlayClipAtPoint(AudioScript.audioObject.getSound("Get"),transform.position);
        if (collision.tag == "Wall")
        {
            if (powerupId == 5 || powerupId == 3)
                GetComponent<BallPowerUp>().modifyBall(collision.gameObject);
            else
                Destroy(gameObject);
        }

    }

    public void ballIsCatched(bool isPlayerTeam, int addScore, float speed, Collider2D you)
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
}
