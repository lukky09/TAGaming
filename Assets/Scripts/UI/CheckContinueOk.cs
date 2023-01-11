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
        int[] numbers = new int[2];
        int i = 0;

        foreach (GameObject item in itemsToCheck)
        {
            textcontent = item.GetComponent<TMP_InputField>().text;
            if (textcontent == "" || int.Parse(textcontent) < 15)
            {
                ok = false;
                break;
            }
            numbers[i] = int.Parse(textcontent);
            i++;
        }
        gameObject.GetComponent<Button>().interactable = ok;
        if (ok)
        {
            SetObjects.initializeSize(numbers[0], numbers[1]);
        }
    }
}
