using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootMechanic : SnowBrawler
{
    [SerializeField] GameObject line1;
    [SerializeField] GameObject line2;
    [SerializeField] float aimAngle;
    [SerializeField] float aimTime;
    public float aimMovementSpeedPerc;

    private float currentaimangle;
    private float currentAimTime;

    // Start is called before the first frame update
    void Start()
    {
        playerteam = true;
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
            line1.SetActive(true);
            line2.SetActive(true);
        }
        if (Input.GetMouseButtonUp(0) && isAiming)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePos - (Vector2)this.transform.position;
            direction = Quaternion.AngleAxis(Random.Range(-(currentaimangle / 2), currentaimangle / 2), Vector3.forward) * direction.normalized;
            shootBall(direction);
            isAiming = false;
            line1.SetActive(false);
            line2.SetActive(false);
            shartShooting();
        }
        if (isAiming)
        {
            Vector3 throwDir = Vector3.Normalize((Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position));
            float x = throwDir.x;
            transform.localScale = new Vector3(Mathf.RoundToInt(x / Mathf.Abs(x)), 1, 1);
            if (transform.localScale.x == -1)
                throwDir = Vector3.Reflect(throwDir, Vector3.right);
            currentAimTime -= Time.deltaTime;
            currentaimangle = (currentAimTime < 0) ? 0 : (currentAimTime / aimTime) * aimAngle;
            line1.GetComponent<LineRenderer>().SetPosition(0, Vector3.zero);
            line1.GetComponent<LineRenderer>().SetPosition(1, Quaternion.Euler(0, 0, currentaimangle * -1 / 2) * throwDir * 20);
            line2.GetComponent<LineRenderer>().SetPosition(0, Vector3.zero);
            line2.GetComponent<LineRenderer>().SetPosition(1, Quaternion.Euler(0, 0, currentaimangle / 2) * throwDir * 20);
        }
    }

}
