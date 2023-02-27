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
    [SerializeField] int ballScore;
    public float aimMovementSpeedPerc;

    public bool isAiming;
    private float currentaimangle;
    private float currentAimTime;
    private TakeSnowBall snowballreference;
    private LineRenderer linemanager;
    private CatchBall ballcatch;

    // Start is called before the first frame update
    void Start()
    {
        playerteam = true;
        id = 0;
        snowballreference = GetComponent<TakeSnowBall>();
        linemanager = line.GetComponent<LineRenderer>();
        currentAimTime = 0;
        isAiming = false;
        ballcatch = GetComponent<CatchBall>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(ballcatch.getBall() == null);
        if (Input.GetMouseButtonDown(0) && (snowballreference.getballamount() > 0 || ballcatch.getBall() != null))
        {
            currentAimTime = aimTime;
            isAiming = true;
            line.SetActive(true);
        }
        if (Input.GetMouseButtonUp(0) && isAiming)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject ballin = Instantiate(ball, (Vector2)this.transform.position, Quaternion.identity);
            Vector2 direction = mousePos - (Vector2)this.transform.position;
            direction = Quaternion.AngleAxis(Random.Range(-(currentaimangle / 2), currentaimangle / 2), Vector3.forward) * direction.normalized;
            ballin.GetComponent<BallMovement>().initialize(ballSpeed, direction, true, ballScore, this.GetComponent<BoxCollider2D>());
            snowballreference.decreaseballamount();
            isAiming = false;
            line.SetActive(false);
        }
        if (isAiming)
        {
            Vector3 pos = Vector3.Normalize((Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position));
            if (transform.localScale.x == -1)
                pos = Vector3.Reflect(pos, Vector3.right);
            currentAimTime -= Time.deltaTime;
            currentaimangle = (currentAimTime < 0) ? 0 : (currentAimTime / aimTime) * aimAngle;
            linemanager.SetPosition(0, Quaternion.Euler(0, 0, currentaimangle * -1 / 2) * pos * 20);
            linemanager.SetPosition(1, Vector3.zero);
            linemanager.SetPosition(2, Quaternion.Euler(0, 0, currentaimangle / 2) * pos * 20);
        }
    }

}
