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
    public void ThrowBallServerRPC(Vector2 ThrowDirection,bool IsThrownBallFromCatching)
    {
        _snowBrawlerRef.shootBall(ThrowDirection);
        ReduceBallAmountClientRPC(IsThrownBallFromCatching);
    }

    [ClientRpc]
    public void ReduceBallAmountClientRPC(bool IsThrownBallFromCatching)
    {
        _snowBrawlerRef.UpdateHoldedBallsAmountAfterThrow(IsThrownBallFromCatching);
    }

    [ServerRpc]
    public void PickUpBallServerRPC()
    {
        int currentBallAmount = _snowBrawlerRef.ballAmount;
        _snowBrawlerRef.getBall();
        if (_snowBrawlerRef.ballAmount > currentBallAmount)
            AddBallAmountClientRPC(_snowBrawlerRef.ballPowerId);
    }

    [ClientRpc]
    public void AddBallAmountClientRPC(int BallPowerID)
    {
        _snowBrawlerRef.UpdateHoldedBallsAmountAfterPickup(BallPowerID);
    }
}
