using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialNPCScript : MonoBehaviour
{
    [SerializeField] GameObject objectToDelete;

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
            return;
        }
        if (GetComponent<BotActions>().canSeeProjectile() && Vector2.Distance(transform.position, GetComponent<BotActions>().getSeenProjectile().transform.position) <= 1)
        {
            GetComponent<BotActions>().tryCatchBall();
            return;
        }

        if (botReference.canSeePerson() && !targetSeen)
        {
            targetSeen = true;
            botReference.setTarget(botReference.getSeenEnemy());
            target = botReference.getSeenEnemy();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(GetComponent<SnowBrawler>().getIsCatching());
        if(!GetComponent<SnowBrawler>().getIsCatching())
            Destroy(objectToDelete);
    }
}
