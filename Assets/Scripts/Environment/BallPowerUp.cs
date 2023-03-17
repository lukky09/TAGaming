using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BallPowerUp : MonoBehaviour
{
    [SerializeField] int pierceScoreAdd;
    [SerializeField] float explosionDelay;
    [SerializeField] float explosionRadius;
    [SerializeField] float movementSpeedSlow;
    [SerializeField] float slowTime;
    [SerializeField] int splitBalls;
    [SerializeField] float splitRange;

    BallMovement bmRef;
    //Utk Powerup Sticky Bomb
    Vector2 distance;
    GameObject collision;
    private void Start()
    {
        bmRef = GetComponent<BallMovement>();
    }

    public void modifyBall(GameObject collider)
    {
        SnowBrawler sbReff = collider.GetComponent<SnowBrawler>();
        switch (bmRef.getBallPowerId())
        {
            //Piercer
            case 1:
                bmRef.addScore(pierceScoreAdd);
                break;
            //Boomerang
            case 2:
                if (bmRef.getPlayerTeam() != sbReff.getplayerteam())
                {
                    bmRef.ballIsCatched(bmRef.getPlayerTeam(), sbReff.ballScoreAdd, sbReff.ballSpeedAdd, collider.GetComponent<BoxCollider2D>());
                    bmRef.setDirection(bmRef.getThrower().transform.position - transform.position);
                }
                else
                    Destroy(gameObject);
                break;
            //Le Bombe
            case 3:
                GetComponent<CircleCollider2D>().enabled = false;
                GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                collision = collider;
                distance = collider.transform.position - transform.position;
                Debug.Log("BeforeWait");
                StartCoroutine(TimedExplode(explosionDelay));
                break;
            //Hu dingin
            case 4:
                sbReff.slowDown(movementSpeedSlow, slowTime);
                Destroy(gameObject);
                break;
            //Tembok?
            case 5:
                Vector2 backPos = (Vector2)transform.position + bmRef.getDirection() * 1.1f;
                float angle = Vector2.SignedAngle((Vector2)transform.position, backPos);
                float dist = Mathf.Sqrt(1 + Mathf.Pow(Mathf.Sin(Mathf.Deg2Rad * angle * 2), 2));
                backPos = (Vector2)transform.position + bmRef.getDirection() *  dist * 1.1f;
                Debug.DrawLine(transform.position, backPos, Color.red, 5);
                Collider2D[] explosiveCollision = Physics2D.OverlapCircleAll(backPos, 0.1f, 64);
                foreach (Collider2D item in explosiveCollision)
                {
                    Debug.Log(item.name);   
                }
                if (explosiveCollision.Length == 0)
                {
                    GameObject[] balls = new GameObject[splitBalls];
                    float initialAngle = -splitRange / 2;
                    for (int i = 0; i < splitBalls; i++)
                    {
                        balls[i] = Instantiate(gameObject, backPos, Quaternion.identity);
                        balls[i].GetComponent<BallMovement>().setDirection(Quaternion.Euler(0, 0, initialAngle + i * (splitRange / (splitBalls - 1))) * bmRef.getDirection());
                        balls[i].GetComponent<BallMovement>().setBallScore(Mathf.CeilToInt(bmRef.getBallScore() / splitBalls));
                        balls[i].GetComponent<BallMovement>().setPowerUpID(0);
                        balls[i].transform.localScale = new Vector3(0.7f, 0.7f, 1);
                        for (int j = 0; j < i; j++)
                            Physics2D.IgnoreCollision(balls[i].GetComponent<CircleCollider2D>(), balls[j].GetComponent<CircleCollider2D>());
                    }
                }

                //Back Trace
                RaycastHit2D line = Physics2D.Linecast(backPos,transform.position);
                Debug.DrawLine(backPos, line.point,Color.black,5);
                Destroy(gameObject);
                break;
        }
    }

    private void FixedUpdate()
    {
        if (bmRef.getBallPowerId() == 3 && collision != null)
            transform.position = (Vector2)collision.transform.position - distance;
    }

    IEnumerator TimedExplode(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Collider2D[] explosiveCollision = Physics2D.OverlapCircleAll(transform.position, explosionRadius, 8);

        int hitPlayers = 0;
        foreach (Collider2D coll in explosiveCollision)
        {
            if (coll.GetComponent<SnowBrawler>().getplayerteam() != bmRef.getPlayerTeam())
                hitPlayers++;
        }
        BarScoreManager.addscore(bmRef.getPlayerTeam(), bmRef.getBallScore() * hitPlayers);
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
