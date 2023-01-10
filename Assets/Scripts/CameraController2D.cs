using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController2D : MonoBehaviour
{
    [SerializeField] GameObject FloorTilemapObject;

    Tilemap FloorTilemapReference;
    float[] tilesize; // width, height
    // Start is called before the first frame update
    void Start()
    {
        FloorTilemapReference = FloorTilemapObject.GetComponent<Tilemap>();
        tilesize = new float[2] { 0, Camera.main.orthographicSize };
        tilesize[0] = tilesize[1] * Camera.main.aspect;
    }

    // Update is called once per frame
    void Update()
    {

        float x = Mathf.Clamp(transform.position.x, tilesize[0], SetStones.getWidth() - tilesize[0]);
        float y = Mathf.Clamp(transform.position.y, tilesize[1], -SetStones.getHeight() + tilesize[1]);
        Debug.Log(transform.position.x + " " + tilesize[0] + " " + (SetStones.getWidth() - tilesize[0]));
        transform.position = new Vector3(x, y);
    }
}
