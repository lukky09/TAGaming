using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GetShotAngle :MonoBehaviour
{
   public async static Task<float> GetAngle(GameObject you, GameObject them, float ballspeed)
    {
        //HUKUM COSINEE!!!!!!!!!
        Vector2 pos1 = them.transform.position;
        await Task.Delay(100);
        Vector2 pos2 = them.transform.position;
        float targetSpeed = Vector2.Distance(pos1, pos2) * 10; // Kali 10 agar harapane isa dapet speed original nya
        float angleBallandTarget = Vector2.SignedAngle(pos2 + pos1, (Vector2)you.transform.position - pos2);
        float initTargetandBallDistance = Vector2.Distance(pos1,you.transform.position);
        float r = targetSpeed / ballspeed;
        double isiAkar = Mathf.Pow(2 * r * Mathf.Cos(angleBallandTarget * Mathf.Deg2Rad), 2) + 4 * (1-r) * initTargetandBallDistance;
        isiAkar = Mathf.Sqrt((float)isiAkar);
        //Ini dapat 2 posibilitas jarak antara asal bola menuju muka target
        double prediksi1 = (-2 * r * Mathf.Cos(angleBallandTarget * Mathf.Deg2Rad) + isiAkar) / (2 - 2 * r);
        double prediksi2 = (-2 * r * Mathf.Cos(angleBallandTarget * Mathf.Deg2Rad) - isiAkar) / (2 - 2 * r);
        //Dari jarak diatas ambil yang jarake lebih pendek
        double prediksiFinal;
        if (prediksi1 < prediksi2 && prediksi1 > 0)
            prediksiFinal = prediksi1;
        else if (prediksi1 < 0 && prediksi2 < 0)
            return 0;
        else
            prediksiFinal = prediksi2;
        // Dari jarak yang didapat diambil waktu
        double time = prediksiFinal / ballspeed;
        // Dari waktu bisa didapat Jarak antara Posisi awal target dan posisi akhir
        double TargetDistance = time * targetSpeed;
        //Dan terakhir dari situ pakai rumus cos lagi untuk dapet jumlah sudut yang diperlukan
        float A = (float)TargetDistance;
        float B = Vector2.Distance(you.transform.position, pos2);
        float C = (float)prediksiFinal;
        float resultAngle = Mathf.Acos((Mathf.Pow(A, 2) + Mathf.Pow(B, 2) - Mathf.Pow(C, 2)) / (2 * A * B));
        return resultAngle * Mathf.Rad2Deg;
        
    }
}
