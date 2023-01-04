using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheckContinueOk : MonoBehaviour
{
    [SerializeField] GameObject[] itemsToCheck;

    public void check()
    {
        bool ok = true;
        string textcontent;
        
        foreach (GameObject item in itemsToCheck)
        {
            textcontent = item.GetComponent<TMP_InputField>().text;
            if (textcontent == "" || int.Parse(textcontent)<15)
            {
                ok = false;
                break;
            }
        }
        gameObject.GetComponent<Button>().interactable = ok;
    }
}
