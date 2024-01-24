using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        NetworkObject networkRef = collider.GetComponent<NetworkObject>();
        ulong colliderNetworkID = networkRef == null ? 0 : networkRef.NetworkObjectId;
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
                BombStickClientRPC(colliderNetworkID, transform.position, distance);
                break;
            //Hu dingin
            case 4:
                GoSlowClientRPC(colliderNetworkID);
                GetComponent<NetworkObject>().Despawn(true);
                break;
            //Tembok?
            case 5:
                Vector2 backPos = (Vector2)transform.position + bmRef.getDirection() * 2f;
                float angle = Vector2.SignedAngle((Vector2)transform.position, backPos);
                float dist = Mathf.Sqrt(1 + Mathf.Pow(Mathf.Sin(Mathf.Deg2Rad * angle * 2), 2));
                backPos = (Vector2)transform.position + bmRef.getDirection() * dist + bmRef.getDirection() * GetComponent<CircleCollider2D>().radius * 1.1f;
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
                    BallMovement movementRef;
                    for (int i = 0; i < splitBalls; i++)
                    {
                        balls[i] = Instantiate(gameObject, backPos, Quaternion.identity);
                        movementRef = balls[i].GetComponent<BallMovement>();
                        movementRef.setDirection(Quaternion.Euler(0, 0, initialAngle + i * (splitRange / (splitBalls - 1))) * bmRef.getDirection());
                        movementRef.setBallScore(Mathf.CeilToInt(bmRef.getBallScore() / 2));
                        movementRef.setPowerUpID(0);
                        balls[i].GetComponent<SpriteRenderer>().sprite = normalBallSprite;
                        balls[i].transform.localScale = new Vector3(0.7f, 0.7f, 1);
                        balls[i].GetComponent<NetworkObject>().Spawn(true);
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
    void BombStickClientRPC(ulong CollidedPlayer, Vector3 ObjectPosition, Vector2 Distance)
    {
        transform.position = ObjectPosition - (Vector3)Distance;
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        if (CollidedPlayer != 0)
        {
            foreach (SnowBrawler Brawler in FindObjectsOfType<SnowBrawler>())
            {
                if (Brawler.GetComponent<NetworkObject>().NetworkObjectId == CollidedPlayer)
                {
                    collision = Brawler.gameObject;
                    break;
                }
            }
        }
        else
        {
            collision = FindObjectOfType<SetObjects>().gameObject;
        }
        distance = Distance;
        StartCoroutine(TimedExplode(explosionDelay));
    }

    [ClientRpc]
    void GoSlowClientRPC(ulong CollidedPlayer)
    {
        NetworkObject brawlerNetRef;
        foreach (SnowBrawler Brawler in FindObjectsOfType<SnowBrawler>())
        {
            brawlerNetRef = Brawler.GetComponent<NetworkObject>();
            if (brawlerNetRef != null && brawlerNetRef.NetworkObjectId == CollidedPlayer)
            {
                Brawler.slowDown(movementSpeedSlow, slowTime);
                break;
            }
        }
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
        foreach (Collider2D coll in explosiveCollision)
        {
            // Nggak isa melakukan coroutine kalau object ilang
            coll.GetComponent<SnowBrawler>().getHit(0.5f, gameObject);
            if (coll.GetComponent<SnowBrawler>().getplayerteam() != bmRef.getPlayerTeam())
            {
                hitPlayers++;
            }
        }
        BarScoreRtc barRef = FindObjectOfType<BarScoreRtc>();
        if (IsServer)
        {
            if(barRef != null)
                barRef.addScoreServerRPC(bmRef.getPlayerTeam(), bmRef.getBallScore() * hitPlayers);
            gameObject.GetComponent<NetworkObject>().Despawn(false);
        }
        Destroy(gameObject);
    }
}
