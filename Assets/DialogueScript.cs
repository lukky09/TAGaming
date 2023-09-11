using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueScript : MonoBehaviour
{
    [SerializeField] string _dialogueText;
    [SerializeField] GameObject _dialogueBox;
    [SerializeField] GameObject _eButton;
    bool _canPress;

    private void Start()
    {
        _canPress = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<SnowBrawler>() == null)
            return;
        _eButton.SetActive(true);
        _canPress = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && _canPress && !_dialogueBox.activeSelf)
        {
            _dialogueBox.SetActive(true);
            _dialogueBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _dialogueText;
        }else if (Input.anyKeyDown && _canPress)
        {
            _dialogueBox.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<SnowBrawler>() == null)
            return;
        _canPress = false;
        _eButton.SetActive(false);
        _dialogueBox.SetActive(false);
    }

}
