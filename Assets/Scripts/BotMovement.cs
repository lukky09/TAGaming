using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotMovement : SnowBrawler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        base.addscore(true,0.1f);
        Destroy(collision);
    }
}
