using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BotActions : MonoBehaviour
{
    public int mapSegmentid;
    public float sidewaysAngle, aimSpeedPercentage;
    public bool debug;
    public Vector2 walkLocation;

    bool isAiming;
    Rigidbody2D thisRigid;
    PlayersManager playerManagerRef;
    GameObject target;
    Vector2 lastpos, direction;
    float timeDelay = 0.5f, currentTimeDelay = 0;
    SnowBrawler snowBrawlerRef;

    private void Start()
    {
        isAiming = false;
        thisRigid = GetComponent<Rigidbody2D>();
        snowBrawlerRef = GetComponent<SnowBrawler>();
    }

    public void forgetTarget()
    {
        target = null;
    }

    public void setIsAiming(bool isAiming)
    {
        this.isAiming = isAiming;
    }

    public void setTarget(GameObject target)
    {
        this.target = target;
    }

    public void setWalkLocation(Coordinate walkLocation)
    {
        this.walkLocation = walkLocation.returnAsVector();
    }

    public void setMapSegmentID(int mapSegmentid, PlayersManager playerManagerRef)
    {
        this.mapSegmentid = mapSegmentid;
        this.playerManagerRef = playerManagerRef;
    }

    private void Update()
    {
        currentTimeDelay -= Time.deltaTime;
        //update posisi sebelumnya target untuk prediksi
        if (target != null && currentTimeDelay <= 0)
        {
            lastpos = target.transform.position;
            currentTimeDelay = timeDelay;
        }
    }

    private void FixedUpdate()
    {
        // koding sini lebih rapi ketimbang di Visual Script
        if (!walkLocation.Equals(Vector2.zero))
        {
            direction = Vector3.Normalize(walkLocation - (Vector2)transform.position);
            thisRigid.MovePosition((Vector2)transform.position + (direction * Time.deltaTime * snowBrawlerRef.runSpeed * (isAiming ? aimSpeedPercentage : 1)));
        }
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
            Debug.Log("Chosen " + (mapSegmentid - 1)+","+ target.ToString());
            return AStarAlgorithm.makeWay(Coordinate.returnAsCoordinate(transform.position), target);
        }
        else
        {
            do
            {
                target = new Coordinate(Random.Range(0, SetObjects.getWidth()+1), Random.Range(0, SetObjects.getHeight()+1));
            } while (AStarAlgorithm.doAstarAlgo(Coordinate.returnAsCoordinate(transform.position), target, SetObjects.getMap(false)) == null);
        }
        return AStarAlgorithm.makeWay(Coordinate.returnAsCoordinate(transform.position), target);
    }
}
