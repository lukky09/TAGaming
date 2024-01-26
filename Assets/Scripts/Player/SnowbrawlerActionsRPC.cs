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
    [SerializeField] RectTransform NameText;
    bool _isPlayer;

    protected void Awake()
    {
        _snowBrawlerRef = GetComponent<SnowBrawler>();
        if (GetComponent<PlayerMovement>() != null)
            _isPlayer = true;
    }

    Vector3 _flipVector;
    protected void FixedUpdate()
    {
        _flipVector = new Vector3(_spriteFlip.Value ? -1 : 1, 1, 1);
        transform.localScale = _flipVector;
        if (_isPlayer)
            NameText.localScale = _flipVector;
    }

    [ServerRpc]
    public void ThrowBallServerRPC(Vector2 ThrowDirection)
    {
        _snowBrawlerRef.ThrowBall(ThrowDirection);
        ulong ballid = _snowBrawlerRef.getCaughtBall() == null ? 0 : _snowBrawlerRef.getCaughtBall().GetComponent<NetworkObject>().NetworkObjectId;
        ThrowBallClientRPC(ballid, _snowBrawlerRef.getBallAmount(), _snowBrawlerRef.ballPowerId, transform.position, ThrowDirection);
    }

    [ClientRpc]
    public void ThrowBallClientRPC(ulong NetworkObjectID, int BallAmount, int BallPowerID, Vector3 ThrowerPosition, Vector2 ThrowDirection)
    {
        _snowBrawlerRef.ThrowCaughtBallClient(NetworkObjectID, ThrowDirection, ThrowerPosition);
        _snowBrawlerRef.UpdateBallAmount(NetworkObjectID, BallAmount, BallPowerID);
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
        _snowBrawlerRef.UpdateBallAmount(NetworkObjectID, BallAmount, BallPowerID);
        _snowBrawlerRef.UpdateHoldedBallsAmountAfterPickup();
    }
}
