using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideandShow : MonoBehaviour
{
    [SerializeField] GameObject itemToDisable;
    [SerializeField] GameObject itemToEnable;

    private void OnDestroy()
    {
        Debug.Log("Huh");
        if (itemToDisable != null)
        itemToDisable.SetActive(false);
        if (itemToEnable != null)
            itemToEnable.SetActive(true);
    }
}
