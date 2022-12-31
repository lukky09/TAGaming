using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootMechanic : SnowBrawler
{
    public GameObject ball;
    public float ballSpeed;
    [SerializeField] GameObject line;
    [SerializeField] float aimAngle;
    [SerializeField] float aimTime;
    [SerializeField] float aimMovementSpeedPerc;

    private bool isAiming;
    private float aliveTime;
    private float currentAimTime;
    private bool fromUserTeam;
    private TakeSnowBall snowballreference;
    private LineRenderer linemanager;

    // Start is called before the first frame update
    void Start()
    {
        initialize();
        playerteam = true;
        snowballreference = GetComponent<TakeSnowBall>();
        //line.transform.position = transform.position;
        linemanager = line.GetComponent<LineRenderer>();
        currentAimTime = 0;
        isAiming = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && snowballreference.getballamount() > 0)
        {
            currentAimTime = aimTime;
            isAiming = true;
            line.SetActive(true);
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject ballin = Instantiate(ball, (Vector2)this.transform.position, Quaternion.identity);
            Vector2 direction = mousePos - (Vector2)this.transform.position;
            direction = direction.normalized;
            ballin.GetComponent<BallMovement>().initialize(ballSpeed, direction, 0);
            snowballreference.decreaseballamount();
        }
        if (Input.GetMouseButtonUp(0))
        {
            isAiming = false;
            line.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (isAiming)
        {
            Vector3 pos = Vector3.Normalize((Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position));
            Debug.Log(transform.localScale.x);
            currentAimTime -= Time.deltaTime;
            float angle = (currentAimTime < 0) ? 0 : (currentAimTime / aimTime) * aimAngle;
            linemanager.SetPosition(0, transform.position + Quaternion.Euler(0, 0, angle * -1 / 2) * pos * 20);
            linemanager.SetPosition(1, transform.position);
            linemanager.SetPosition(2, transform.position + Quaternion.Euler(0, 0, angle / 2) * pos * 20);
        }
    }
}
