using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveDirection;
    Rigidbody2D thisRigid;
    ShootMechanic SMReference;
    SnowBrawler SBReference;

    // Start is called before the first frame update
    void Start()
    {
        moveDirection = new Vector2(0,0);
        thisRigid = this.GetComponent<Rigidbody2D>();
        SMReference = this.GetComponent<ShootMechanic>();
        SBReference = this.GetComponent<SnowBrawler>();
        thisRigid.useFullKinematicContacts = true;
    }

    // Update is called once per frame
    void Update()
    {
        float diagonalCheck = Mathf.Sqrt(Mathf.Pow(Input.GetAxisRaw("Horizontal"), 2) + Mathf.Pow(Input.GetAxisRaw("Vertical"), 2));
        moveDirection.x = Input.GetAxisRaw("Horizontal") * SBReference.runSpeed ;
        moveDirection.y = Input.GetAxisRaw("Vertical") * SBReference.runSpeed;
        if (diagonalCheck != 0f)
            moveDirection /= diagonalCheck;
        if (SMReference.isAiming)
            moveDirection *= SMReference.aimMovementSpeedPerc;
        if (Input.GetAxisRaw("Horizontal") != 0 && !PauseGame.isPaused)
            transform.localScale = new Vector3(Input.GetAxisRaw("Horizontal"), 1, 1);
    }

    private void FixedUpdate()
    {
        if (GetComponent<SnowBrawler>().getCatchTimer() < 0)
            thisRigid.MovePosition((Vector2)this.transform.position + moveDirection * Time.deltaTime);
    }

}
