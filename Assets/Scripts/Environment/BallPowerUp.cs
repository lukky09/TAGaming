using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BallPowerUp : NetworkBehaviour
{
    [SerializeField] int pierceScoreAdd;
    [SerializeField] Sprite normalBallSprite;
    [SerializeField] float explosionDelay;
    [SerializeField] float explosionRadius;
    [SerializeField] float movementSpeedSlow;
    [SerializeField] float slowTime;
    [SerializeField] int splitBalls;
    [SerializeField] float splitRange;

    float particleTimer;

    BallMovement bmRef;
    //Utk Powerup Sticky Bomb
    Vector2 distance;
    GameObject collision;
    private void Awake()
    {
        particleTimer = explosionDelay;
        bmRef = GetComponent<BallMovement>();
    }

    public void modifyBall(GameObject collider)
    {
        SnowBrawler sbReff = collider.GetComponent<SnowBrawler>();
        ulong colliderNetworkID = collider.GetComponent<NetworkObject>().NetworkObjectId;
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
                    bmRef.ballIsCatched(bmRef.getPlayerTeam(), sbReff.ballScoreAdd, sbReff.ballSpeedAdd, collider.GetComponent<BoxCollider2D>(), bmRef.getThrower());
                    bmRef.setDirection(bmRef.getThrower().transform.position - transform.position);
                    BoomerangUpdateClientRPC(colliderNetworkID);
                }
                else
                    GetComponent<NetworkObject>().Despawn();
                break;
            //Le Bombe
            case 3:
                GetComponent<CircleCollider2D>().enabled = false;
                GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                collision = collider;
                distance = collider.transform.position - transform.position;
                StartCoroutine(TimedExplode(explosionDelay));
                BombStickClientRPC(colliderNetworkID);
                break;
            //Hu dingin
            case 4:
                sbReff.slowDown(movementSpeedSlow, slowTime);
                Destroy(gameObject);
                break;
            //Tembok?
            case 5:
                Vector2 backPos = (Vector2)transform.position + bmRef.getDirection() * 2f;
                float angle = Vector2.SignedAngle((Vector2)transform.position, backPos);
                float dist = Mathf.Sqrt(1 + Mathf.Pow(Mathf.Sin(Mathf.Deg2Rad * angle * 2), 2));
                backPos = (Vector2)transform.position + bmRef.getDirection() * dist * 1.1f;
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
                        //balls[i].GetComponent<BallMovement>().initialize();
                        balls[i].GetComponent<BallMovement>().setDirection(Quaternion.Euler(0, 0, initialAngle + i * (splitRange / (splitBalls - 1))) * bmRef.getDirection());
                        balls[i].GetComponent<BallMovement>().setBallScore(Mathf.CeilToInt(bmRef.getBallScore() / 2));
                        balls[i].GetComponent<BallMovement>().setPowerUpID(0);
                        balls[i].GetComponent<SpriteRenderer>().sprite = normalBallSprite;
                        balls[i].transform.localScale = new Vector3(0.7f, 0.7f, 1);
                        for (int j = 0; j < i; j++)
                            Physics2D.IgnoreCollision(balls[i].GetComponent<CircleCollider2D>(), balls[j].GetComponent<CircleCollider2D>());
                    }
                }

                //Back Trace
                RaycastHit2D line = Physics2D.Linecast(backPos, transform.position);
                Debug.DrawLine(backPos, line.point, Color.black, 5);
                Destroy(gameObject);
                break;
        }
    }

    [ClientRpc]
    void BoomerangUpdateClientRPC(ulong PlayerID)
    {
        Debug.Log(bmRef.getThrower().name);
        foreach (SnowBrawler Brawler in FindObjectsOfType<SnowBrawler>())
        {
            if (Brawler.GetComponent<NetworkObject>().NetworkObjectId == PlayerID)
            {
                transform.position = Brawler.transform.position;
                break;
            }
        }
        bmRef.setDirection(bmRef.getThrower().transform.position - transform.position);
    }

    [ClientRpc]
    void BombStickClientRPC(ulong CollidedPlayer)
    {
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        foreach (SnowBrawler Brawler in FindObjectsOfType<SnowBrawler>())
        {
            if(Brawler.GetComponent<NetworkObject>().NetworkObjectId == CollidedPlayer)
            {
                collision = Brawler.gameObject;
                break;
            }
        }
        distance = collision.transform.position - transform.position;
        StartCoroutine(TimedExplode(explosionDelay));
    }

    private void Update()
    {
        if (GetComponent<BallMovement>().getBallPowerId() == 3 && particleTimer <= 0.3f)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        //biar bom bisa lekat ke target
        if (bmRef.getBallPowerId() == 3 && collision != null)
        {
            particleTimer -= Time.deltaTime;
            transform.position = (Vector2)collision.transform.position - distance;
        }
    }

    IEnumerator TimedExplode(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Collider2D[] explosiveCollision = Physics2D.OverlapCircleAll(transform.position, explosionRadius, 8);

        int hitPlayers = 0;
        //Debug.Log("Boom");
        foreach (Collider2D coll in explosiveCollision)
        {
            // Nggak isa melakukan coroutine kalau object ilang
            coll.GetComponent<SnowBrawler>().getHit(0.5f,gameObject);
            if (coll.GetComponent<SnowBrawler>().getplayerteam() != bmRef.getPlayerTeam())
                hitPlayers++;
        }
        FindObjectOfType<BarScoreRtc>().addScoreServerRPC(bmRef.getPlayerTeam(), bmRef.getBallScore() * hitPlayers);
        Destroy(gameObject);

    }
}
