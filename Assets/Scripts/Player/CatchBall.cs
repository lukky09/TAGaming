using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchBall : MonoBehaviour
{
    SnowBrawler brawlerReference;

    private void Start()
    {
        brawlerReference = GetComponent<SnowBrawler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && brawlerReference.getCatchTimer() < 0)
        {
            brawlerReference.catchBall();
        }
        
    }
}
