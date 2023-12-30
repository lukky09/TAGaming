using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveDirection;
    Rigidbody2D thisRigid;
    ShootMechanic SMReference;
    SnowBrawler SBReference;
    SpriteRenderer SRReference;
  
    
    float _lastPosX;

    // Start is called before the first frame update
    void Start()
    {
        moveDirection = new Vector2(0, 0);
        thisRigid = GetComponent<Rigidbody2D>();
        SMReference = GetComponent<ShootMechanic>();
        SBReference = GetComponent<SnowBrawler>();
        SRReference = GetComponent<SpriteRenderer>();
        thisRigid.useFullKinematicContacts = true;


    }

    // Update is called once per frame
    void Update()
    {

        if (!SBReference.canAct)
        {
            moveDirection = new Vector2(0, 0);
            return;
        }
        float diagonalCheck = Mathf.Sqrt(Mathf.Pow(Input.GetAxisRaw("Horizontal"), 2) + Mathf.Pow(Input.GetAxisRaw("Vertical"), 2));
        moveDirection.x = Input.GetAxisRaw("Horizontal") * SBReference.runSpeed;
        moveDirection.y = Input.GetAxisRaw("Vertical") * SBReference.runSpeed;
        if (diagonalCheck != 0f)
            moveDirection /= diagonalCheck;
        if (SMReference.isAiming)
            moveDirection *= SMReference.aimMovementSpeedPerc;
    }

    private void FixedUpdate()
    {
        thisRigid.MovePosition((Vector2)this.transform.position + moveDirection * Time.deltaTime);
        SRReference.flipX = MathF.Abs(transform.position.x - _lastPosX) > 0.1f ? (transform.position.x - _lastPosX) < -0 : SRReference.flipX;
        _lastPosX = transform.position.x;
    }

}
