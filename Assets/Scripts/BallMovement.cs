using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallMovement : MonoBehaviour
{
    private float speed;
    private Vector2 direction;
    Rigidbody2D thisRigid;

    // Start is called before the first frame update
    void Start()
    {
        thisRigid = this.GetComponent<Rigidbody2D>();
    }

    public void initialize(float speed, Vector2 direction)
    {
        this.speed = speed;
        this.direction = direction;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        thisRigid.MovePosition((Vector2)this.transform.position + direction * Time.deltaTime * speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
