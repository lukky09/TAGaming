using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallMovement : MonoBehaviour
{
    private float speed;
    private Vector2 direction;
    private bool fromPlayerTeam;
    private int ballScore;
    Rigidbody2D thisRigid;

    // Start is called before the first frame update
    void Start()
    {
        thisRigid = this.GetComponent<Rigidbody2D>();
    }

    public void initialize(float speed, Vector2 direction, bool isPlayerTeam,int ballScore)
    {
        this.speed = speed;
        this.direction = direction;
        this.fromPlayerTeam = isPlayerTeam;
    }
    public bool getPlayerTeam()
    {
        return fromPlayerTeam;
    }

    public void delet()
    {
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        thisRigid.MovePosition((Vector2)this.transform.position + direction * Time.deltaTime * speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Wall")
            Destroy(gameObject);
    }

    public void ballIsCatched(bool isPlayerTeam,int addScore)
    {
        this.fromPlayerTeam = isPlayerTeam;
        this.ballScore += addScore;
    }
}
