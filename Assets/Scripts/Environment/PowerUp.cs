using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PowerUp : NetworkBehaviour
{
    [SerializeField] float spawnTime;
    [SerializeField] Material playerMaterial;
    [SerializeField] Color materialColor;
    [SerializeField] int startingValue;
    [SerializeField] int ValueRange;
    [SerializeField] bool randomPowerup;
    [SerializeField] Sprite[] powerUpSprites;


    int powerUpValue;
    float currentSpawnTime;
    GameObject ball;

    // Start is called before the first frame update
    void Start()
    {
        ball = transform.GetChild(0).gameObject;
        Material m = new Material(playerMaterial);
        m.SetColor("_OutlineColor", materialColor);
        m.SetFloat("_OutlineThickness", 1);
        if (!IsServer && !SceneManager.GetActiveScene().name.Equals("Tutorial"))
        {
            RequestBallPowerUpUpdateServerRPC(new ServerRpcParams());
            return;
        }
        if (startingValue > ValueRange || startingValue < 0 || randomPowerup)
            powerUpValue = Random.Range(1, ValueRange + 1);
        else
            powerUpValue = startingValue;
        ball.GetComponent<SpriteRenderer>().sprite = powerUpSprites[powerUpValue - 1];
        currentSpawnTime = 0;
    }

    private void Update()
    {
        if (!IsServer)
            return;
        currentSpawnTime -= Time.deltaTime;
        if (currentSpawnTime <= 0 && !ball.activeSelf)
        {
            GetComponent<Animator>().speed = 1;
            ball.SetActive(true);
            if (randomPowerup)
            {
                powerUpValue = Random.Range(1, ValueRange + 1);
                ball.GetComponent<SpriteRenderer>().sprite = powerUpSprites[powerUpValue - 1];
            }
            SpawnPowerupBallClientRPC(powerUpValue, new ClientRpcParams());
        }
    }

    public bool isActive()
    {
        return currentSpawnTime <= 0;
    }

    public (Sprite, int) getPowerupId()
    {
        if (currentSpawnTime > 0)
            return (null, 0);
        ball.SetActive(false);
        currentSpawnTime = spawnTime;
        GetComponent<Animator>().speed = 0;
        if (IsServer)
            DisableSpecialBallClientRPC();
        return (powerUpSprites[powerUpValue - 1], powerUpValue);
    }

    public Sprite getPowerUpSprite(int PowerID)
    {
        return powerUpSprites[PowerID - 1];
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestBallPowerUpUpdateServerRPC(ServerRpcParams SRPCParams)
    {
        SpawnPowerupBallClientRPC(powerUpValue, new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { SRPCParams.Receive.SenderClientId } } });
    }

    [ClientRpc]
    public void SpawnPowerupBallClientRPC(int PowerID, ClientRpcParams CRPCParams)
    {
        currentSpawnTime = 0;
        GetComponent<Animator>().speed = 1;
        ball.SetActive(true);
        powerUpValue = PowerID;
        ball.GetComponent<SpriteRenderer>().sprite = powerUpSprites[PowerID - 1];
    }

    [ClientRpc]
    public void DisableSpecialBallClientRPC()
    {
        ball.SetActive(false);
        currentSpawnTime = spawnTime;
        GetComponent<Animator>().speed = 0;
    }
}
