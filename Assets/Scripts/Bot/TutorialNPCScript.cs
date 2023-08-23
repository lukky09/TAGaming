using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialNPCScript : MonoBehaviour
{
    BotActions botReference;
    bool targetSeen;
    GameObject target;

    void Start()
    {
        targetSeen = false;
        botReference = GetComponent<BotActions>();
        botReference.setDirection(new Vector2(-1, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null && target.GetComponent<ShootMechanic>().IsFaking && GetComponent<BotActions>().getCatchTimer() < 0)
        {
            GetComponent<BotActions>().tryCatchBall();
        }
        if (botReference.canSeePerson() && !targetSeen)
        {
            targetSeen = true;
            botReference.setTarget(botReference.getSeenEnemy());
            target = botReference.getSeenEnemy();
        }

    }
}
