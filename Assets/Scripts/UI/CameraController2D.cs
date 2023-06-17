using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController2D : MonoBehaviour
{
    //[SerializeField] GameObject FloorTilemapObject;
    [SerializeField] GameObject ObjectToFollow;
    [SerializeField] bool freeFollow;

    //Tilemap FloorTilemapReference;
    float[] camSize; // width, height
    Transform followedObjetTransform;
    // Start is called before the first frame update
    void Start()
    {
        camSize = new float[2] { 0, Camera.main.orthographicSize };
        camSize[0] = camSize[1] * Camera.main.aspect;
        followedObjetTransform = ObjectToFollow.transform;
    }

    public void setCameraFollower(GameObject anobject,bool free)
    {
        followedObjetTransform = anobject.transform;
        freeFollow = free;
    }

    // Update is called once per frame
    void Update()
    {
        if (!freeFollow)
        {
            //ini harus ada ukuran levelnya biar g glitchy
            float x = Mathf.Clamp(followedObjetTransform.position.x, camSize[0], SetObjects.getWidth() + 2 - camSize[0]);
            float y = Mathf.Clamp(followedObjetTransform.position.y, -SetObjects.getHeight() - 2 + camSize[1] + 1, -camSize[1] + 1);
            transform.position = new Vector3(x, y, -10);
        }
        else
        {
            transform.position = new Vector3(followedObjetTransform.position.x, followedObjetTransform.position.y, -10);
        }
    }
}
