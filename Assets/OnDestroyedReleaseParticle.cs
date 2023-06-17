using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class OnDestroyedReleaseParticle : MonoBehaviour
{
    [SerializeField] GameObject particle;

    private void OnDestroy()
    {
        Instantiate(particle, transform.position, quaternion.identity);
    }
}
