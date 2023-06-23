using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorManager : MonoBehaviour
{
    public DropdownAuto[] dropdowns = new DropdownAuto[4];
    public int[] dropdownChoices = new int[4];
    public  Material shaderMaterial;

    private void Awake()
    {
        List<string> colors = new List<string>();
        for (int i = 0; i < 5; i++)
        {
            colors.Add(pilihanWarna.getWarna(i).getName());
        }
        foreach (DropdownAuto item in dropdowns)
        {
            item.setColorManager(this, colors);
        }
        register();
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

public static class pilihanWarna
{
    public static Warna getWarna(int id)
    {
        switch (id)
        {
            case 0: return new Warna("Red", Color.red);
            case 1: return new Warna("Green", Color.green);
            case 2: return new Warna("Blue", Color.blue);
            case 3: return new Warna("Black", Color.black);
            case 4: return new Warna("Purple", Color.magenta);
            default: return new Warna("White", Color.white);
        }
    }
}
