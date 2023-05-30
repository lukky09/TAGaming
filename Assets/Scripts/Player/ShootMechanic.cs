using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootMechanic : SnowBrawler
{
    [SerializeField] GameObject line;
    [SerializeField] float aimAngle;
    [SerializeField] float aimTime;
    public float aimMovementSpeedPerc;

    private float currentaimangle;
    private float currentAimTime;
    private LineRenderer linemanager;

    // Start is called before the first frame update
    void Start()
    {
        playerteam = true;
        linemanager = line.GetComponent<LineRenderer>();
        currentAimTime = 0;
        isAiming = false;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.E))
        {
            getBall();
        }
        if (Input.GetMouseButtonDown(0) && (ballAmount > 0 || caughtBall != null))
        {
            currentAimTime = aimTime;
            isAiming = true;
            line.SetActive(true);
        }
        if (Input.GetMouseButtonUp(0) && isAiming)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePos - (Vector2)this.transform.position;
            direction = Quaternion.AngleAxis(Random.Range(-(currentaimangle / 2), currentaimangle / 2), Vector3.forward) * direction.normalized;
            shootBall(direction);
            isAiming = false;
            line.SetActive(false);
            shartShooting();
        }
        if (isAiming)
        {
            Vector3 pos = Vector3.Normalize((Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position));
            float x = pos.x - transform.position.x;
            transform.localScale = new Vector3(Mathf.RoundToInt(x / Mathf.Abs(x)), 1, 1);
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
