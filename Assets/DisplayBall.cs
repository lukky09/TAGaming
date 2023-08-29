using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayBall : MonoBehaviour
{
    GameObject _ballContainer, _backImage, _frontImage;
    SnowBrawler _snowBrawlerReference;

    // Start is called before the first frame update
    void Start()
    {
        //Komputasinya berat tapi mek sekali so yeh
        _ballContainer = GameObject.Find("ItemBorder");
        _backImage = _ballContainer.transform.GetChild(0).gameObject;
        _frontImage = _ballContainer.transform.GetChild(1).gameObject;
        _snowBrawlerReference = GetComponent<SnowBrawler>();
    }

    public void updateUI()
    {
        if (_snowBrawlerReference.getCaughtBall() != null && _snowBrawlerReference.getOnlyGroundBallAmount() > 0)
        {
            _backImage.SetActive(true);
            _backImage.GetComponent<Image>().sprite = _snowBrawlerReference.getBallSprite();
            _frontImage.SetActive(true);
            _frontImage.GetComponent<Image>().sprite = _snowBrawlerReference.getCaughtBall().GetComponent<SpriteRenderer>().sprite;
            _frontImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(10, -10);
        }
        else if (_snowBrawlerReference.getCaughtBall() != null)
        {
            _backImage.SetActive(false);
            _frontImage.SetActive(true);
            _frontImage.GetComponent<Image>().sprite = _snowBrawlerReference.getCaughtBall().GetComponent<SpriteRenderer>().sprite;
            _frontImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }
        else if (_snowBrawlerReference.getBallAmount() > 0)
        {
            _backImage.SetActive(false);
            _frontImage.SetActive(true);
            _frontImage.GetComponent<Image>().sprite = _snowBrawlerReference.getBallSprite();
            _frontImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }
        else
        {
            _backImage.SetActive(false);
            _frontImage.SetActive(false);
        }
    }
}
