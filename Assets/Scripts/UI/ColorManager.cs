using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorManager : MonoBehaviour
{
    public Warna[] listWarna = new Warna[5];
    public DropdownAuto[] dropdowns = new DropdownAuto[4];
    public int[] dropdownChoices = new int[4];
    public  Material shaderMaterial;

    private void Awake()
    {
        if (listWarna.Length == 0)
            return;
        if (listWarna[0] == null)
            initializeColors();
        List<string> colors = new List<string>();
        foreach (Warna w in listWarna)
            colors.Add(w.getName());
        foreach (DropdownAuto item in dropdowns)
        {
            item.setColorManager(this, colors);
        }
        register();
    }

    public void initializeColors()
    {
        listWarna[0] = new Warna("Red", Color.red);
        listWarna[1] = new Warna("Green", Color.green);
        listWarna[2] = new Warna("Blue", Color.blue);
        listWarna[3] = new Warna("Black", Color.black);
        listWarna[4] = new Warna("Purple", Color.magenta);
    }

    public static void mainGameDefault()
    {
        PlayerPrefs.SetInt("DD0", 4);
        PlayerPrefs.SetInt("DD1", 2);
        PlayerPrefs.SetInt("DD2", 0);
        PlayerPrefs.SetInt("DD3", 1);
        PlayerPrefs.Save();
    }

    public void register()
    {
        if (PlayerPrefs.HasKey("DD0"))
        {
            Debug.Log("Ada");
            dropdownChoices[0] = PlayerPrefs.GetInt("DD0");
            dropdownChoices[1] = PlayerPrefs.GetInt("DD1");
            dropdownChoices[2] = PlayerPrefs.GetInt("DD2");
            dropdownChoices[3] = PlayerPrefs.GetInt("DD3");
        }
        else
        {
            dropdownChoices[0] = 4;
            dropdownChoices[1] = 2;
            dropdownChoices[2] = 0;
            dropdownChoices[3] = 1;
            saveSetting();
        }
        refreshChoices();
    }

    void refreshChoices()
    {
        for (int i = 0; i < 4; i++)
            dropdowns[i].setColor(dropdownChoices[i]);
    }

    public void updateChanges(int ddIndex, int choice)
    {
        if (dropdownChoices[ddIndex] == choice)
            return;
        int storedChoice = dropdownChoices[ddIndex];
        dropdownChoices[ddIndex] = choice;
        for (int i = 0; i < dropdownChoices.Length; i++)
        {
            if (dropdownChoices[i] == choice && i != ddIndex)
            {
                dropdownChoices[i] = storedChoice;
                dropdowns[i].setColor(dropdownChoices[i]);
                
                break;
            }
        }
    }

    public void saveSetting()
    {
        PlayerPrefs.SetInt("DD0", dropdownChoices[0]);
        PlayerPrefs.SetInt("DD1", dropdownChoices[1]);
        PlayerPrefs.SetInt("DD2", dropdownChoices[2]);
        PlayerPrefs.SetInt("DD3", dropdownChoices[3]);
        PlayerPrefs.Save();
    }

}

public class Warna{

    string name;
    Color color;

    public Warna(string name, Color color)
    {
        this.name = name;
        this.color = color;
    }

    public string getName()
    {
        return name;
    }

    public Color getColor()
    {
        return color;
    }

}
