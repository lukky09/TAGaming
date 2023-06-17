using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitTriggerDissapear : MonoBehaviour
{
    [SerializeField] GameObject objectToDelete;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            Object.Destroy(objectToDelete);
            GetComponent<Animator>().Play("Base Layer.Dummy");
        }
    }

    public void backToNormal()
    {
        GetComponent<Animator>().Play("Base Layer.Idle");
    }
}
