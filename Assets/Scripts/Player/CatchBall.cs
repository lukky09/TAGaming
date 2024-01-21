using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CatchBall : NetworkBehaviour
{
    SnowBrawler brawlerReference;

    private void Start()
    {
        brawlerReference = GetComponent<SnowBrawler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && brawlerReference.canAct && !brawlerReference.isAiming && brawlerReference.canCatchBall && IsOwner)
        {
            brawlerReference.tryCatch();
        }
        
    }
}
