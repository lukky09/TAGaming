using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    float spawnTime;
    float currentSpawnTime;
    GameObject ball;
    [SerializeField]
    Material playerMaterial;
    [SerializeField]
    Color materialColor;

    // Start is called before the first frame update
    void Start()
    {
        Material m = new Material(playerMaterial);
        m.SetColor("_OutlineColor", materialColor);
        m.SetFloat("_OutlineThickness", 5);
        ball = transform.GetChild(0).gameObject;
        ball.GetComponent<SpriteRenderer>().material = m;
        currentSpawnTime = spawnTime;
    }

    private void Update()
    {
        currentSpawnTime -= Time.deltaTime;
        if (currentSpawnTime <= 0)
            ball.SetActive(true);
        
    }

    public bool isActive()
    {
        return currentSpawnTime <= 0;
    }

    public int getPowerupId()
    {
        ball.SetActive(false);
        currentSpawnTime = spawnTime;
        return 1; 
    }
}
