using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    Vector2 moveDirection;
    Rigidbody2D thisRigid;
    ShootMechanic SMReference;
    SnowBrawler SBReference;
    SpriteRenderer SRReference;
    int _SpawnID;

    // Start is called before the first frame update
    void Start()
    {
        moveDirection = new Vector2(0, 0);
        thisRigid = this.GetComponent<Rigidbody2D>();
        SMReference = this.GetComponent<ShootMechanic>();
        SBReference = this.GetComponent<SnowBrawler>();
        SRReference = this.GetComponent<SpriteRenderer>();
        thisRigid.useFullKinematicContacts = true;
        if (IsOwner)
            onlineBehavior();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!SBReference.canAct || !IsOwner)
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
        if (Input.GetAxisRaw("Horizontal") != 0 && !PauseGame.isPaused && !SMReference.isAiming)
            SRReference.flipX = Input.GetAxisRaw("Horizontal") < 0;
    }

    private void FixedUpdate()
    {
        thisRigid.MovePosition((Vector2)this.transform.position + moveDirection * Time.deltaTime);
    }

    void onlineBehavior()
    {
        Debug.Log("OnlineTime");
        _SpawnID = 1;
        if (LobbyManager.instance.IsOnline)
        {
            int thisJoinOrder = 0;
            string isLeftTeam = "";
            //Ambil data player ini
            foreach (Player p in LobbyManager.instance.CurrentLobby.Players)
                if (p.Id == LobbyManager.instance.PlayerID)
                {
                    thisJoinOrder = Int32.Parse(p.Data["joinOrder"].Value);
                    isLeftTeam = p.Data["isLeftTeam"].Value;
                    break;
                }
            //Liat berapa order yang lebih kecil dari player
            foreach (Player p in LobbyManager.instance.CurrentLobby.Players)
                if (Int32.Parse(p.Data["joinOrder"].Value) < thisJoinOrder && p.Data["isLeftTeam"].Value.Equals(isLeftTeam))
                    _SpawnID++;
            //Dan juga ganti tim kalau tim kanan
            if (isLeftTeam.Equals("n"))
            {
                SBReference.playerteam = false;
                GetComponent<ColorTaker>().updateColor(1);
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                SBReference.playerteam = true;
                GetComponent<ColorTaker>().updateColor(0);
                GetComponent<SpriteRenderer>().flipX = false;
            }
        }
        transform.position = FindObjectOfType<SetObjects>().GetPositionFromOrderID(_SpawnID, SBReference.playerteam);

        //transform.SetParent(GameObject.Find("Players").transform);
        if (IsOwner)
            FindObjectOfType<CameraController2D>().setCameraFollower(gameObject, false);
    }

    [ServerRpc]
    void testingServerRpc()
    {

    }

    [ClientRpc]
    void testingClientRpc()
    {

    }

}
