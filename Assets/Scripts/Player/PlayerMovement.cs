using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveDirection;
    public float speed;
    Animator animator;
    Rigidbody2D thisRigid;
    ShootMechanic SMReference;
    CatchBall CBReference;

    // Start is called before the first frame update
    void Start()
    {
        moveDirection = new Vector2(0,0);
        animator = this.GetComponent<Animator>();
        thisRigid = this.GetComponent<Rigidbody2D>();
        SMReference = this.GetComponent<ShootMechanic>();
        CBReference = GetComponent<CatchBall>();
        thisRigid.useFullKinematicContacts = true;
    }

    // Update is called once per frame
    void Update()
    {
        float diagonalCheck = Mathf.Sqrt(Mathf.Pow(Input.GetAxisRaw("Horizontal"), 2) + Mathf.Pow(Input.GetAxisRaw("Vertical"), 2));
        moveDirection.x = Input.GetAxisRaw("Horizontal") * speed ;
        moveDirection.y = Input.GetAxisRaw("Vertical") * speed;
        if (diagonalCheck != 0f)
            moveDirection /= diagonalCheck;
        if (SMReference.isAiming)
            moveDirection *= SMReference.aimMovementSpeedPerc;
        if (Input.GetAxisRaw("Horizontal") != 0 && !PauseGame.isPaused)
            transform.localScale = new Vector3(Input.GetAxisRaw("Horizontal"), 1, 1);
        animator.SetInteger("yMove", (int)Input.GetAxisRaw("Vertical"));
        animator.SetBool("xMove", Input.GetAxisRaw("Horizontal") != 0);
    }

    private void FixedUpdate()
    {
        if(CBReference.getCatchTime()<0)
            thisRigid.MovePosition((Vector2)this.transform.position + moveDirection * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.transform.name);
    }

}
