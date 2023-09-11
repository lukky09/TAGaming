using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTaker : MonoBehaviour
{
    public int id;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("DD0"))
        {
            GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", pilihanWarna.getWarna(PlayerPrefs.GetInt("DD" + id)).getColor());
        }
        else
            Debug.LogWarning($"Id {id} tidak punya warna");
    }

}
