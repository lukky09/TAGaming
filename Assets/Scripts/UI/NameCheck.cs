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
    [SerializeField] GameObject _lobbyLayer;
    [SerializeField] TMP_InputField _nameField;
    [SerializeField] TextMeshProUGUI _nameText;
    private static string _multiplayerName;
    public static string MultiplayerName { get { return _multiplayerName; } set { _multiplayerName = value; } }

    private void Start()
    {
        if (PlayerPrefs.HasKey("MPName"))
        {
            _multiplayerName = PlayerPrefs.GetString("MPName");
        }
        if (_multiplayerName == null)
        {
            foreach (Transform childObject in _layersContainer.transform)
            {
                childObject.gameObject.SetActive(false);
            }
            _nameLayer.SetActive(true);
        }
        else
        {
            foreach (Transform childObject in _layersContainer.transform)
            {
                childObject.gameObject.SetActive(false);
            }
            _lobbyLayer.SetActive(true);
        }
    }

    public void nameBackButtonCheck(GameObject multiplayerActionLayer)
    {
        if (_multiplayerName == null)
        {
            SceneManager.LoadScene(0);
            return;
        }
        _nameLayer.SetActive(true);
        multiplayerActionLayer.SetActive(true);
        gameObject.SetActive(true);
    }

    public void saveName(GameObject openedLayer)
    {
        if (_nameField.text.Replace(" ", string.Empty).Length != 0)
        {
            _multiplayerName = _nameField.text;
            _nameText.text = _multiplayerName;
        }
        else
            _multiplayerName = "Player#" + UnityEngine.Random.Range(1, 10000).ToString();
        _nameField.text = "";
        openedLayer.SetActive(true);
        _nameLayer.SetActive(false);
        //onNameChanged.Invoke(_multiplayerReference.MultiplayerName);
    }

}
