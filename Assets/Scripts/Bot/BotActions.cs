using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BotActions : MonoBehaviour
{
    [SerializeField] float timeLimit;
    GameObject target;
    Vector2 lastpos;
    float timeDelay = 0.5f, currentTimeDelay = 0;

    public void setTarget(GameObject target)
    {
        this.target = target;
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

    public Vector2 GetAngle(GameObject them, float ballspeed)
    {
        //HUKUM COSINEE!!!!!!!!!
        Vector2 pos1 = lastpos;
        Vector2 pos2 = them.transform.position;
        float targetSpeed = Vector2.Distance(pos1, pos2) / (timeDelay - currentTimeDelay);
        float angleBallandTarget = Vector2.SignedAngle(Vector3.Normalize((Vector2)transform.position - pos1),Vector3.Normalize(pos2 - pos1));
        float initTargetandBallDistance = Vector2.Distance(pos2,transform.position);
        Debug.Log("Speed,Angle dan Jarak Awal : "+targetSpeed + "," + angleBallandTarget + "," + initTargetandBallDistance);
        Debug.DrawLine((Vector2)transform.position,pos1,Color.red,5);
        Debug.DrawLine(pos2, pos1, Color.red, 5);
        float r = targetSpeed / ballspeed;
        //Ini dapat 2 posibilitas jarak antara asal bola menuju muka target
        float a = 1 - Mathf.Pow(r,2);
        float b = 2 * Mathf.Cos(angleBallandTarget * Mathf.Deg2Rad) * r;
        float c = - Mathf.Pow(initTargetandBallDistance,2);
        Debug.Log("abc : " + a + "," + b + "," + c);
        double isiAkar = Mathf.Pow(b, 2) - 4 * a * c;
        isiAkar = Mathf.Sqrt((float)isiAkar);
        double prediksi1 = (-b + isiAkar) / (2 * a);
        double prediksi2 = (-b - isiAkar) / (2 * a);
        Debug.Log("Prediksi Jarak 1 & 2 : " + prediksi1 + "," + prediksi2);
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
        
        //Debug.Log("prediksiFinal : " + prediksiFinal);
        //// Dari waktu bisa didapat Jarak antara Posisi awal target dan posisi akhir
        //double TargetDistance = time * targetSpeed;
        ////Dan terakhir dari situ pakai rumus cos lagi untuk dapet jumlah sudut yang diperlukan
        //float I = (float)TargetDistance;
        //float J = initTargetandBallDistance;
        //float K = (float)prediksiFinal;
        //Debug.Log("IJK : " + I + "," + J + "," + K);
        //float resultAngle = Mathf.Acos((Mathf.Pow(K, 2) + Mathf.Pow(J, 2) - Mathf.Pow(I, 2)) / (2 * K * J)) * Mathf.Rad2Deg;
        //if (resultAngle == float.NaN)
        //{
        //    Debug.Log("Result NAN");
        //    return Vector3.Normalize(pos2 - (Vector2)transform.position);
        //}
        //Debug.Log($"{Vector3.Normalize(pos2 - (Vector2)transform.position)} jadi {Quaternion.Euler(0, 0, resultAngle) * Vector3.Normalize(pos2 - (Vector2)transform.position)} , ({resultAngle})");
        //return Quaternion.Euler(0, 0, resultAngle) * Vector3.Normalize(pos2 - (Vector2)transform.position);
    }
}
