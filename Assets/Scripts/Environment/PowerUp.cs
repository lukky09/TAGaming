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
    [SerializeField] Sprite[] powerUpSprites;


    int powerUpValue;
    float currentSpawnTime; 
    GameObject ball;

    // Start is called before the first frame update
    void Start()
    {
        if (startingValue > ValueRange || startingValue < 0 || randomPowerup)
            powerUpValue = Random.Range(1, startingValue + 1);
        else
            powerUpValue = startingValue;
        Material m = new Material(playerMaterial);
        m.SetColor("_OutlineColor", materialColor);
        m.SetFloat("_OutlineThickness", 1);
        ball = transform.GetChild(0).gameObject;
        ball.GetComponent<SpriteRenderer>().sprite = powerUpSprites[powerUpValue-1];

        currentSpawnTime = 0;
    }

    private void Update()
    {
        currentSpawnTime -= Time.deltaTime;
        if (currentSpawnTime <= 0 && !ball.activeSelf)
        {
            GetComponent<Animator>().speed = 1;
            ball.SetActive(true);
            if (randomPowerup)
            {
                powerUpValue = Random.Range(1, startingValue + 1);
                ball.GetComponent<SpriteRenderer>().sprite = powerUpSprites[powerUpValue - 1];
            }
        }

    }

    public bool isActive()
    {
        return currentSpawnTime <= 0;
    }

    public (Sprite,int) getPowerupId()
    {
        if (currentSpawnTime > 0)
            return (null,0);
        ball.SetActive(false);
        currentSpawnTime = spawnTime;
        GetComponent<Animator>().speed = 0;
        return (powerUpSprites[powerUpValue - 1],powerUpValue);
    }
}
