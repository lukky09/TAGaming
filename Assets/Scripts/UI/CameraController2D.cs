using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController2D : MonoBehaviour
{
    //[SerializeField] GameObject FloorTilemapObject;
    [SerializeField] GameObject ObjectToFollow;

    //Tilemap FloorTilemapReference;
    float[] tilesize; // width, height
    Transform followedObjetTransform;
    // Start is called before the first frame update
    void Start()
    {
        //FloorTilemapReference = FloorTilemapObject.GetComponent<Tilemap>();
        tilesize = new float[2] { 0, Camera.main.orthographicSize };
        tilesize[0] = tilesize[1] * Camera.main.aspect;
        followedObjetTransform = ObjectToFollow.transform;
    }

    public void setCameraFollower(GameObject anobject)
    {
        followedObjetTransform = anobject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        //ini harus ada ukuran levelnya biar g glitchy
        float x = Mathf.Clamp(followedObjetTransform.position.x, tilesize[0], SetObjects.getWidth() - tilesize[0]);
        float y = Mathf.Clamp(followedObjetTransform.position.y, -SetObjects.getHeight() + tilesize[1] + 1, -tilesize[1] + 1);
        transform.position = new Vector3(x, y, -10);
    }
}