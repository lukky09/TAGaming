using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BotActions : MonoBehaviour
{
    int mapSegmentid;
    float sidewaysAngle;
    bool debug;
    Vector2 walkLocation;
    float searchTimer, catchTimer;
    [SerializeField] float aimSpeedPercentage;

    //untuk vision dan nembak
    [SerializeField] int linecastAmount;
    [SerializeField] float linecastAngle;
    [SerializeField] float castingLength;
    [SerializeField] float sphereCastRadius;
    [SerializeField] float catchTimerDelay;
    [SerializeField] float catchChance;
    GameObject sawBallGO, sawEnemyGO, sawProjectileGO;

    [SerializeField] GameObject noticeMark;
    Rigidbody2D thisRigid;
    PlayersManager playerManagerRef;
    GameObject target;
    Vector2 lastpos, direction,viewDirection;
    float timeDelay = 0.5f, currentTimeDelay = 0;
    SnowBrawler snowBrawlerRef;

    private void Start()
    {
        searchTimer = 0;
        catchTimer = 0;
        thisRigid = GetComponent<Rigidbody2D>();
        snowBrawlerRef = GetComponent<SnowBrawler>();
    }

    public void forgetTarget()
    {
        target = null;
    }

    public void setIsAiming(bool isAiming)
    {
        snowBrawlerRef.isAiming = isAiming;
    }

    public void setSearchTimer(float searchTimer)
    {
        this.searchTimer = searchTimer;
    }

    public GameObject getTarget()
    {
        return target;
    }

    public void setTarget(GameObject target)
    {
        this.target = target;
        StartCoroutine(visualiseNotice());
    }

    public void setWalkLocation(Coordinate walkLocation)
    {
        this.walkLocation = walkLocation.returnAsVector();
    }

    public void setWalkLocation(Vector2 walkLocation)
    {
        this.walkLocation = walkLocation;
    }

    public void setMapSegmentID(int mapSegmentid, PlayersManager playerManagerRef)
    {
        this.mapSegmentid = mapSegmentid;
        this.playerManagerRef = playerManagerRef;
    }

    private void Update()
    {
        searchTimer -= Time.deltaTime;
        currentTimeDelay -= Time.deltaTime;
        catchTimer -= Time.deltaTime;
        //update posisi sebelumnya target untuk prediksi
        if (target != null && currentTimeDelay <= 0)
        {
            lastpos = target.transform.position;
            currentTimeDelay = timeDelay;
        }

    }

    private void FixedUpdate()
    {
        sawBallGO = null; sawEnemyGO = null; sawProjectileGO = null;
        RaycastHit2D currentHitObject;
        float initialAngle = -linecastAngle / 2;
        Vector2 currentDirection;
        float shortestBallDist = 999, shortestEnemyDist = 999;
        float currDistance;
        for (int i = 0; i < linecastAmount; i++)
        {
            currentDirection = Quaternion.Euler(0, 0, initialAngle + i * (linecastAngle / (linecastAmount - 1))) * direction;
            currentHitObject = Physics2D.Linecast((Vector2)transform.position + currentDirection / 2, (Vector2)transform.position + currentDirection * castingLength);

            //Kalau keliatan objek
            if (currentHitObject)
            {
                currDistance = Vector2.Distance(transform.position, currentHitObject.collider.transform.position);
                if (currentHitObject.collider.CompareTag("BallPile") && currDistance < shortestBallDist)
                {
                    if (currentHitObject.collider.GetComponent<PowerUp>() == null)
                    {
                        shortestBallDist = currDistance;
                        sawBallGO = currentHitObject.collider.gameObject;
                    }
                    else if (!currentHitObject.collider.GetComponent<PowerUp>().isActive())
                    {
                        shortestBallDist = currDistance;
                        sawBallGO = currentHitObject.collider.gameObject;
                    }
                }
                else if ((currentHitObject.collider.CompareTag("Player") || currentHitObject.collider.CompareTag("EnemyTeam") )&& currDistance < shortestEnemyDist)
                {
                    if (currentHitObject.collider.GetComponent<SnowBrawler>().getplayerteam() != snowBrawlerRef.getplayerteam())
                    {
                        shortestEnemyDist = currDistance;
                        sawEnemyGO = currentHitObject.collider.gameObject;
                    }
                }
                else if (currentHitObject.collider.CompareTag("Projectile"))
                {
                    sawProjectileGO = currentHitObject.collider.gameObject;
                }
            }
            Debug.DrawLine((Vector2)transform.position + currentDirection / 2, (Vector2)transform.position + currentDirection * castingLength, UnityEngine.Color.black, 0.0f);
        }

        if (searchTimer > 0 || !snowBrawlerRef.canAct)
            return;
        // koding sini lebih rapi ketimbang di Visual Script
        if (!walkLocation.Equals(Vector2.zero))
        {
            direction = Vector3.Normalize(walkLocation - (Vector2)transform.position);
            viewDirection = direction;
            
            thisRigid.MovePosition((Vector2)transform.position + (direction * Time.deltaTime * snowBrawlerRef.runSpeed * (snowBrawlerRef.isAiming ? aimSpeedPercentage : 1)));
        }

        float x = (viewDirection.x == 0 ? 1 : viewDirection.x);
        transform.localScale = new Vector3(x / Mathf.Abs(x), transform.localScale.y, transform.localScale.z);

        if (snowBrawlerRef.isAiming)
            direction = Vector3.Normalize((Vector2)target.transform.position - (Vector2)transform.position);
    }

    public void walkSideways()
    {
        float angleOfChoice = ((Random.Range(0, 2) * 2) - 1) * 90;
        angleOfChoice = angleOfChoice + Random.Range(-sidewaysAngle, sidewaysAngle + 1);
        walkLocation = Quaternion.Euler(0, 0, angleOfChoice) * (Vector2)(target.transform.position - transform.position);
    }

    public Vector2 GetAngle(GameObject them, float ballspeed)
    {
        //HUKUM COSINEE!!!!!!!!!
        Vector2 pos1 = lastpos;
        Vector2 pos2 = them.transform.position;
        float targetSpeed = Vector2.Distance(pos1, pos2) / (timeDelay - currentTimeDelay);
        float angleBallandTarget = Vector2.SignedAngle(Vector3.Normalize((Vector2)transform.position - pos1), Vector3.Normalize(pos2 - pos1));
        float initTargetandBallDistance = Vector2.Distance(pos2, transform.position);

        float r = targetSpeed / ballspeed;
        //Ini dapat 2 posibilitas jarak antara asal bola menuju muka target
        float a = 1 - Mathf.Pow(r, 2);
        float b = 2 * Mathf.Cos(angleBallandTarget * Mathf.Deg2Rad) * r;
        float c = -Mathf.Pow(initTargetandBallDistance, 2);

        double isiAkar = Mathf.Pow(b, 2) - 4 * a * c;
        isiAkar = Mathf.Sqrt((float)isiAkar);
        double prediksi1 = (-b + isiAkar) / (2 * a);
        double prediksi2 = (-b - isiAkar) / (2 * a);
        if (debug)
        {
            Debug.Log("Speed,Angle dan Jarak Awal : " + targetSpeed + "," + angleBallandTarget + "," + initTargetandBallDistance);
            Debug.DrawLine((Vector2)transform.position, pos1, Color.red, 5);
            Debug.DrawLine(pos2, pos1, Color.red, 5);
            Debug.Log("abc : " + a + "," + b + "," + c);
            Debug.Log("Prediksi Jarak 1 & 2 : " + prediksi1 + "," + prediksi2);
        }

        //Dari jarak diatas ambil yang jarake lebih pendek
        double prediksiFinal;
        if (double.IsNaN(prediksi1) && double.IsNaN(prediksi2))
            return Vector3.Normalize(pos2 - (Vector2)transform.position);
        else if (!double.IsNaN(prediksi1) && !double.IsNaN(prediksi2))
        {
            if (prediksi1 > 0 && (prediksi2 < 0 || prediksi1 < prediksi2))
                prediksiFinal = prediksi1;
            else if (prediksi2 > 0 && (prediksi1 < 0 || prediksi2 < prediksi1))
                prediksiFinal = prediksi2;
            else
                return Vector3.Normalize(pos2 - (Vector2)transform.position);
        }
        else
        {
            if (double.IsNaN(prediksi1))
                prediksiFinal = prediksi1;
            else
                prediksiFinal = prediksi2;
            if (prediksiFinal < 0)
                return Vector3.Normalize(pos2 - (Vector2)transform.position);
        }
        // Dari jarak yang didapat diambil waktu
        double time = prediksiFinal / ballspeed;
        return Vector3.Normalize((float)time * targetSpeed * (Vector2)Vector3.Normalize(pos2 - pos1) + pos2 - (Vector2)transform.position);
    }

    public Coordinate[] getWaytoRandomCoordinate()
    {
        Coordinate target;
        if (mapSegmentid > 0)
        {
            target = playerManagerRef.getRandomSpot(mapSegmentid - 1);
            //Debug.Log("Chosen " + (mapSegmentid - 1)+","+ target.ToString());
            return AStarAlgorithm.makeWay(Coordinate.returnAsCoordinate(transform.position), target);
        }
        else
        {
            do
            {
                target = new Coordinate(Random.Range(0, SetObjects.getWidth() + 1), Random.Range(0, SetObjects.getHeight() + 1));
            } while (AStarAlgorithm.doAstarAlgo(Coordinate.returnAsCoordinate(transform.position), target, SetObjects.getMap(false)) == null);
        }
        return AStarAlgorithm.makeWay(Coordinate.returnAsCoordinate(transform.position), target);
    }

    public bool stillCanSeeTarget()
    {
        Vector2 direction = target.transform.position - transform.position;
        RaycastHit2D seenObject = Physics2D.Linecast((Vector2)transform.position + direction, target.transform.position);
        return (seenObject.collider.gameObject == target);
    }

    public bool isThrowBlocked()
    {
        Vector2 direction = target.transform.position - transform.position;
        RaycastHit2D[] seenObject = Physics2D.CircleCastAll((Vector2)transform.position, AStarAlgorithm.circleSize, direction, Vector2.Distance(target.transform.position, transform.position));
        foreach (RaycastHit2D item in seenObject)
        {
            if (!item.collider.CompareTag("BallPile") && item.collider.gameObject != target && item.collider.gameObject != gameObject)
                return true;
        }
        return false;
    }

    public void tryCatchBall()
    {
        if (Random.Range(1, 101) < catchChance)
            StartCoroutine(snowBrawlerRef.catchBall());
        catchTimer = catchTimerDelay;
        Debug.Log("Coba Tangkap Bola");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, AStarAlgorithm.circleSize);
    }

    public bool predictProjectileWillHit()
    {
        if (sawProjectileGO == null)
            return false;
        Vector2 dir = Vector3.Normalize(transform.position - sawProjectileGO.transform.position);
        RaycastHit2D[] objectsInWay = Physics2D.CircleCastAll(sawProjectileGO.transform.position, 1, dir, Vector2.Distance(transform.position, sawProjectileGO.transform.position));
        foreach (RaycastHit2D item in objectsInWay)
        {
            if (item.collider.CompareTag("Wall"))
                return false;
            if (item.collider == gameObject)
                return true;
        }
        return false;
    }

    IEnumerator visualiseNotice()
    {
        noticeMark.SetActive(true);
        noticeMark.GetComponent<Animator>().Play("Base Layer.BotNotice");
        yield return new WaitForSeconds(0.5f);
        noticeMark.SetActive(false);
    }

    public bool canSeeProjectile()
    {
        return sawProjectileGO != null;
    }


    public bool canSeeBall()
    {
        return sawBallGO != null;
    }

    public bool canSeePerson()
    {
        return sawEnemyGO != null;
    }

    public GameObject getSeenBall()
    {
        return sawBallGO;
    }

    public GameObject getSeenEnemy()
    {
        return sawEnemyGO;
    }

    public float getCatchTimer()
    {
        return catchTimer;
    }
}
