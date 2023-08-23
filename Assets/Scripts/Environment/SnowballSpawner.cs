using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballSpawner : MonoBehaviour
{
    [SerializeField] GameObject snowball;
    [SerializeField] GameObject snowballContainer;
    [SerializeField] SnowBallManager snowballManager;
    [SerializeField] float timerDelay;
   
    float currentDelay;

    // Start is called before the first frame update
    void Start()
    {
        currentDelay = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currentDelay -= Time.deltaTime;
        bool adaBola = false;
        if (currentDelay <=0)
        {
            foreach (Transform item in snowballContainer.transform)
                if(Vector2.Distance(item.position,transform.position) < 0.5)
                    adaBola = true;
            if (!adaBola)
            {
                currentDelay = timerDelay;
                snowballManager.addBallinVector(transform.position);
            }
        }
    }
}
