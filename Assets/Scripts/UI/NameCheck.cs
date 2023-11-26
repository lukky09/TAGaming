using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NameCheck : MonoBehaviour
{
    [SerializeField] GameObject _layersContainer;
    [SerializeField] GameObject _nameLayer;
    [SerializeField] TMP_InputField _nameField;
    [SerializeField] MultiplayerManager _multiRef;

    public delegate void nameEvent(string name);
    public event nameEvent onNameChanged;
    private void Start()
    {
        if (PlayerPrefs.HasKey("MPName"))
        {
            _multiRef.MultiplayerName = PlayerPrefs.GetString("MPName");
        }
        if (_multiRef.MultiplayerName == null)
        {
            foreach (Transform childObject in _layersContainer.transform)
            {
                childObject.gameObject.SetActive(false);
            }
            _nameLayer.SetActive(true);
        }
    }

    public void nameBackButtonCheck(GameObject multiplayerActionLayer)
    {
        if (_multiRef.MultiplayerName == null)
        {
            SceneManager.LoadScene(0);
            return;
        }
        multiplayerActionLayer.SetActive(true);
        gameObject.SetActive(true);
    }

    public void saveName(GameObject openedLayer)
    {
        if (_nameField.text.Replace(" ", string.Empty).Length != 0)
            _multiRef.MultiplayerName = _nameField.text;
        else
            _multiRef.MultiplayerName = "Player#" + UnityEngine.Random.Range(1,10000).ToString();
        _nameField.text = "";
        openedLayer.SetActive(true);
        _nameLayer.SetActive(false);
        //onNameChanged.Invoke(_multiplayerReference.MultiplayerName);
    }

}
