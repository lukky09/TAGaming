using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchBall : MonoBehaviour
{
    [SerializeField] KeyCode catchButton;
    [SerializeField] float catchTime;
    [SerializeField] int scoreAdd;
    float currentCatchTime;
    GameObject caughtSnowBall;

    private void Start()
    {
        currentCatchTime = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            if (currentCatchTime > 0)
            {
                caughtSnowBall = collision.gameObject;
                caughtSnowBall.GetComponent<BallMovement>().ballIsCatched(this.GetComponent<SnowBrawler>().getplayerteam(),scoreAdd);
                Debug.Log("Nangkap");
            }
            else
            {
                BallMovement bol = collision.gameObject.GetComponent<BallMovement>();
                if (!bol.getPlayerTeam())
                    BarScoreManager.addscore(false, 10);
            }
            Destroy(collision.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(catchButton))
        {
            currentCatchTime = catchTime;
        }
        currentCatchTime -= Time.deltaTime;
    }

    public float getCatchTime()
    {
        return currentCatchTime;
    }

    public GameObject getBall()
    {
        return caughtSnowBall;
    }

    public void deleteBall()
    {
        caughtSnowBall = null;
    }
}
