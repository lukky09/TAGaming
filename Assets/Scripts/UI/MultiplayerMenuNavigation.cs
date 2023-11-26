using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerMenuNavigation : MonoBehaviour
{
    // Start is called before the first frame update
    public void changeScreen(GameObject menuScreen)
    {
        gameObject.SetActive(false);
        menuScreen.SetActive(true);
    }

}
