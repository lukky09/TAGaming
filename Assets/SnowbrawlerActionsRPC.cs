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

}
