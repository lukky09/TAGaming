using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchBall : MonoBehaviour
{
    [SerializeField] KeyCode catchButton;
    SnowBrawler brawlerReference;

    private void Start()
    {
        brawlerReference = GetComponent<SnowBrawler>();
        Debug.Log(brawlerReference == null); 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(catchButton) && brawlerReference.getCatchTimer() < 0)
        {
            brawlerReference.catchBall();
        }
        
    }
}
