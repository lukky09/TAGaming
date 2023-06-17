using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCamera : MonoBehaviour
{
    [SerializeField] GameObject currentCamera;
    [SerializeField] float roomWidth;
    [SerializeField] float roomsStart;

    float[] tilesize;
    // Start is called before the first frame update
    void Start()
    {
        tilesize = new float[2] { 0, Camera.main.orthographicSize };
        tilesize[0] = tilesize[1] * Camera.main.aspect;
    }

    // Update is called once per frame
    void Update()
    {
        float x = Mathf.FloorToInt((transform.position.x - roomsStart) / roomWidth) * roomWidth + (roomWidth / 2) + roomsStart;
        currentCamera.transform.position = new Vector3(x,1,-10);
    }
}
