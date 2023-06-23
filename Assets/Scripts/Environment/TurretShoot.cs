using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretShoot : MonoBehaviour
{

    [SerializeField] Vector2 direction;
    [SerializeField] GameObject snowBall;
    [SerializeField] float shootDelay;
    float shootTimer;

    private void Start()
    {
        shootTimer = shootDelay;
    }


    // Update is called once per frame
    void Update()
    {
        if (shootTimer <= 0)
        {
            shootTimer = shootDelay;
            GetComponent<Animator>().Play("Base Layer.TurretShoot");
        }
        shootTimer -= Time.deltaTime;
        
    }

    public void createBall()
    {
        GameObject tempBall = Instantiate(snowBall, transform.position, Quaternion.identity);
        tempBall.GetComponent<BallMovement>().initialize(10, Vector3.Normalize(direction), false, 1, null, gameObject);
        tempBall.GetComponent<ColorTaker>().id = 2;
    }
}
