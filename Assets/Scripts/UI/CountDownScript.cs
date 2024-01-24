using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class CountDownScript : MonoBehaviour
{
    PlayerManager _playerManagerRefl;
    TextMeshProUGUI _textReference;
    [SerializeField] BarScoreManager _bsObject;
    bool _beginCountdown;
    float _currentCountdownTimer;

    // Start is called before the first frame update

    public void startCounting(PlayerManager playerManagerReff, float Time)
    {
        _textReference = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _playerManagerRefl = playerManagerReff;
        _currentCountdownTimer = Time;
        _beginCountdown = true;
        _textReference.gameObject.SetActive(true);
        _textReference.text = "";

    }

    private void Update()
    {
        if (!_beginCountdown)
            return;
        _currentCountdownTimer -= Time.deltaTime;
        switch (_currentCountdownTimer)
        {
            case <= 4 and >= 1:
                _textReference.text = (Mathf.CeilToInt(_currentCountdownTimer) - 1).ToString();
                break;
            case > 0 and < 1:
                _textReference.text = "GO";
                break;
            case <= 0:
                _playerManagerRefl.setPlayerScriptActive(true);
                _bsObject.StartTimer = true;
                Destroy(gameObject);
                break;
        }
    }
}
