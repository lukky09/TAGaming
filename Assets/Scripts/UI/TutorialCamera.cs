using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCamera : MonoBehaviour
{
    [SerializeField] GameObject currentCamera;
    [SerializeField] float roomWidth;
    [SerializeField] float roomsStart;
    [SerializeField] float transitionLength;

    float currentLimit;
    float currentTransitionTime;
    float[] tilesize;
    // Start is called before the first frame update
    void Start()
    {
        tilesize = new float[2] { 0, Camera.main.orthographicSize };
        tilesize[0] = tilesize[1] * Camera.main.aspect;
        currentLimit = roomsStart;
        currentTransitionTime = -1;
        float x = Mathf.FloorToInt((transform.position.x - roomsStart) / roomWidth) * roomWidth + (roomWidth / 2) + roomsStart;
        currentCamera.transform.position = new Vector3(x, 1, -10);
    }

    // Update is called once per frame
    void Update()
    {
        float x = Mathf.FloorToInt((transform.position.x - roomsStart) / roomWidth) * roomWidth + (roomWidth / 2) + roomsStart;
        if (currentTransitionTime < transitionLength && currentTransitionTime > -1)
        {
            float addedlength = Mathf.Sin((currentTransitionTime * Mathf.PI) / 2) * roomWidth;
            currentCamera.transform.position = new Vector3(x - roomWidth + addedlength, 1, -10);
            currentTransitionTime += Time.unscaledDeltaTime;
            return;
        }
        else if (currentTransitionTime >= transitionLength)
        {
            currentTransitionTime = -1;
            Time.timeScale = 1;
            currentCamera.transform.position = new Vector3(x, 1, -10);
        }
        if (x != currentCamera.transform.position.x)
        {
            Time.timeScale = 0;
            currentTransitionTime = 0;
            currentCamera.transform.position = new Vector3(x, 1, -10);
            currentLimit = x + 0.2f - roomWidth / 2;
            transform.position = new Vector3(transform.position.x + 0.2f, 1, 1);
        }

        transform.position = new Vector3(transform.position.x > currentLimit ? transform.position.x : currentLimit, transform.position.y, 1);
    }
}
