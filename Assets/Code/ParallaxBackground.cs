using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform player; 
    public float parallaxFactor = 0.3f; 

    private Vector3 lastPlayerPosition;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        lastPlayerPosition = player.position;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = player.position - lastPlayerPosition;
        transform.position += deltaMovement * parallaxFactor;

        lastPlayerPosition = player.position;
    }
}
