using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsStartingTransitionOK : MonoBehaviour
{
    [SerializeField] GameObject _startingTransition; 

    // Start is called before the first frame update
    void Start()
    {
        if (MainMenuNavigation.startingTransitionActivated)
        {
            _startingTransition.SetActive(true);
            MainMenuNavigation.startingTransitionActivated = false;
        }
    }

    
}
