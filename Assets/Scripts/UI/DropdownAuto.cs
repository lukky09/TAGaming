using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DropdownAuto : MonoBehaviour
{
    [SerializeField] int ddindex;
    [SerializeField] GameObject coloredObject;
    ColorManager colorManager;
    TMP_Dropdown dd;

    // Start is called before the first frame update
    void Awake()
    {
        dd = GetComponent<TMP_Dropdown>(); 
        float height = GetComponent<RectTransform>().sizeDelta.y;
        transform.GetChild(2).GetComponent<ScrollRect>().scrollSensitivity = 5;
        transform.GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(0, height * 3);
        Transform content = transform.GetChild(2).GetChild(0).GetChild(0);
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, height);
        content.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(0, height);
        content.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().fontSize = transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize;
    }

    public void setColorManager(ColorManager colorManager, List<string> colors)
    {
        this.colorManager = colorManager;
        dd.ClearOptions();
        dd.AddOptions(colors);
    }

    public void setColor(int index)
    {
        dd.value = index;
        Material m = new Material(colorManager.shaderMaterial);
        m.SetColor("_OutlineColor", pilihanWarna.getWarna(index).getColor());
        coloredObject.GetComponent<SpriteRenderer>().material = m;
    }

    public void changeColor()
    {
        colorManager.updateChanges(ddindex, dd.value);
        Material m = new Material(colorManager.shaderMaterial);
        m.SetColor("_OutlineColor", pilihanWarna.getWarna(dd.value).getColor());
        coloredObject.GetComponent<SpriteRenderer>().material = m;
    }

}
