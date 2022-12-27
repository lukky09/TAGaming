using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootMechanic : SnowBrawler
{
    public GameObject ball;
    public float ballSpeed;
    private float aliveTime;
    private bool fromUserTeam;

    // Start is called before the first frame update
    void Start()
    {
        initialize();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0)) {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject ballin = Instantiate(ball, (Vector2)this.transform.position, Quaternion.identity);
            Vector2 direction = mousePos - (Vector2)this.transform.position;
            direction = direction.normalized;
            ballin.GetComponent<BallMovement>().initialize(ballSpeed, direction,0);
        }
    }

    private void FixedUpdate()
    {


    }
}
