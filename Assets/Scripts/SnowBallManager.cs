using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBallManager : MonoBehaviour
{
    public static ArrayList snowballs;
    public GameObject snowballscontainer;

    // Start is called before the first frame update
    void Start()
    {
        snowballs = new ArrayList();
        foreach (Transform ballz in snowballscontainer.GetComponentsInChildren<Transform>())
            snowballs.Add(ballz.gameObject);
        snowballs.RemoveAt(0);
    }

    public static void destroyball(int index)
    {
        print(index);
        Destroy((GameObject)snowballs[index]);
        snowballs.RemoveAt(index);
    }

}
