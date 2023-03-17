using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] float spawnTime;
    [SerializeField] Material playerMaterial;
    [SerializeField] Color materialColor;
    [SerializeField] int startingValue;
    [SerializeField] int ValueRange;
    [SerializeField] bool randomPowerup;

    int powerUpValue;
    float currentSpawnTime; 
    GameObject ball;

    // Start is called before the first frame update
    void Start()
    {
        if (startingValue > ValueRange || startingValue < 0)
            powerUpValue = Random.Range(1, startingValue + 1);
        else
            powerUpValue = startingValue;
        Material m = new Material(playerMaterial);
        m.SetColor("_OutlineColor", materialColor);
        m.SetFloat("_OutlineThickness", 5);
        ball = transform.GetChild(0).gameObject;
        ball.GetComponent<SpriteRenderer>().material = m;
        currentSpawnTime = 0;
    }

    private void Update()
    {
        currentSpawnTime -= Time.deltaTime;
        if (currentSpawnTime <= 0)
        {
            ball.SetActive(true);
            if(randomPowerup)
                powerUpValue = Random.Range(1, startingValue + 1);
        }

    }

    public bool isActive()
    {
        return currentSpawnTime <= 0;
    }

    public int getPowerupId()
    {
        ball.SetActive(false);
        if (currentSpawnTime > 0)
            return 0;
        currentSpawnTime = spawnTime;
        return powerUpValue;
    }
}
