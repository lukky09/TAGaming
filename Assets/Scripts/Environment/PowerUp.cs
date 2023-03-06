using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    float spawnTime;
    float currentSpawnTime;
    Color disColor;

    // Start is called before the first frame update
    void Start()
    {
        currentSpawnTime = spawnTime;
        disColor = this.GetComponent<SpriteRenderer>().color;
    }

    private void Update()
    {
        currentSpawnTime -= Time.deltaTime;
        if (currentSpawnTime <= 0)
            this.GetComponent<SpriteRenderer>().color = disColor;
        
    }

    public bool isActive()
    {
        return currentSpawnTime <= 0;
    }

    public int getPowerupId()
    {
        this.GetComponent<SpriteRenderer>().color = new Color(disColor.r, disColor.g, disColor.b, disColor.a / 2);
        currentSpawnTime = spawnTime;
        return 1; 
    }
}
