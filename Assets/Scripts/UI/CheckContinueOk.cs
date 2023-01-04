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
        int[] numbers = new int[2] { 0,0};
        int i = 0;

        foreach (GameObject item in itemsToCheck)
        {
            textcontent = item.GetComponent<TMP_InputField>().text;
            if (textcontent == "" || int.Parse(textcontent) < 15)
            {
                numbers[i] = int.Parse(textcontent);
                i++;
                ok = false;
                break;
            }
        }
        gameObject.GetComponent<Button>().interactable = ok;
        if (ok)
        {
            Debug.Log(numbers[0] + "," + numbers[1]);
            SetStones.initializeSize(numbers[0], numbers[1]);
        }
    }
}
