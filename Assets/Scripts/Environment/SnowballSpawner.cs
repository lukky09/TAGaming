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
    int initialNumber;

    // Start is called before the first frame update
    void Start()
    {
        initialNumber = snowballContainer.transform.childCount;
        currentDelay = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (snowballContainer.transform.childCount > initialNumber)
            return;
        currentDelay -= Time.deltaTime;
        if (currentDelay <=0 && transform.childCount==0)
        {
            currentDelay = timerDelay;
            snowballManager.addBallinVector(transform.position);
        }
    }
}
