using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NameText : MonoBehaviour
{
    [SerializeField] NameCheck _nameReference;
    [SerializeField] MultiplayerManager _multiRef;

    void changeName(string ewe)
    {
        
    }

    private void Start()
    {
        //_nameReference.onNameChanged += changeName;
    }

    private void OnEnable()
    {
        GetComponent<TextMeshProUGUI>().text = _multiRef.MultiplayerName;
    }

}
