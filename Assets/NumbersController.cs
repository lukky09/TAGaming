using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumbersController : MonoBehaviour
{
    GameObject _puluhan, _satuan;
    [SerializeField] Sprite[] _angka;
    [SerializeField] float _timeAlive;
    float _currentTimeAlive;
    Vector2 _direction;
    Vector2 _startingPosition;
    public Vector2 StartingPosition { set { _startingPosition = value; } }

    // Start is called before the first frame update
    void Awake()
    {
        _currentTimeAlive = _timeAlive;
        _puluhan = transform.GetChild(0).gameObject;
        _satuan = transform.GetChild(1).gameObject;
        //Pilih arah acak untuk terbang
        _direction = new Vector2(Random.Range(-1f,1f), Random.Range(-1f, 1f));
    }

    public void setGambar(int skor)
    {
        if (skor >= 10)
            _puluhan.GetComponent<SpriteRenderer>().sprite = _angka[skor / 10];
        else
            _puluhan.SetActive(false);
        _satuan.GetComponent<SpriteRenderer>().sprite = _angka[skor % 10];
    }

    // Update is called once per frame
    void Update()
    {
        //Normalisasi waktu utk function
        float time = (_timeAlive - _currentTimeAlive) / _timeAlive;

        float c4 = (2 * Mathf.PI) / 3;

        float distance =  time == 0 ? 0 : time == 1 ? 1 : Mathf.Pow(2, -10 * time) * Mathf.Sin((float)((time * 10 - 0.75) * c4)) + 1;
        transform.position = _startingPosition + _direction * distance;

        _currentTimeAlive -= Time.deltaTime;

        //Kalau waktu abis self destruct
        if (_currentTimeAlive <= 0)
            Destroy(gameObject);
    }
}
