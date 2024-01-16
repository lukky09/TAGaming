using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SnowbrawlerActionsRPC : NetworkBehaviour
{
    public bool SpriteFlip { set { _spriteFlip.Value = value; } get { return _spriteFlip.Value; } }
    public SnowBrawler SnowBrawlerRef { get { return _snowBrawlerRef; } }

    NetworkVariable<bool> _spriteFlip = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    SnowBrawler _snowBrawlerRef;

    protected void Awake()
    {
        _snowBrawlerRef = GetComponent<SnowBrawler>();
    }

    protected void FixedUpdate()
    {
        transform.localScale = new Vector3(_spriteFlip.Value ? -1 : 1, 1, 1);
    }

    [ServerRpc]
    public void ThrowBallServerRPC(Vector2 ThrowDirection)
    {
        _snowBrawlerRef.ThrowBall(ThrowDirection);
        ulong ballid = _snowBrawlerRef.getCaughtBall() == null ? 0 : _snowBrawlerRef.getCaughtBall().GetComponent<NetworkObject>().NetworkObjectId;
        ReduceBallAmountClientRPC(ballid, _snowBrawlerRef.getBallAmount(), _snowBrawlerRef.ballPowerId);
    }

    [ClientRpc]
    public void ReduceBallAmountClientRPC(ulong NetworkObjectID, int BallAmount, int BallPowerID)
    {
        _snowBrawlerRef.UpdateBallAmount(NetworkObjectID, BallAmount, BallPowerID, false);
    }

    [ServerRpc]
    public void PickUpBallServerRPC()
    {
        int currentBallAmount = _snowBrawlerRef.ballAmount;
        _snowBrawlerRef.PickUpBallFromGround();
        ulong ballid = _snowBrawlerRef.getCaughtBall() == null ? 0 : _snowBrawlerRef.getCaughtBall().GetComponent<NetworkObject>().NetworkObjectId;
        if (_snowBrawlerRef.ballAmount > currentBallAmount)
            AddBallAmountClientRPC(ballid, _snowBrawlerRef.getBallAmount(), _snowBrawlerRef.ballPowerId);
    }

    [ClientRpc]
    public void AddBallAmountClientRPC(ulong NetworkObjectID, int BallAmount, int BallPowerID)
    {
        _snowBrawlerRef.UpdateBallAmount(NetworkObjectID, BallAmount, BallPowerID, false);
        _snowBrawlerRef.UpdateHoldedBallsAmountAfterPickup();
    }
}
