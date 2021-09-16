using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    Transform player;
    float x;
    float y;
    public bool setCameraX;
    public bool setCameraY;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void LateUpdate()
    {
        if (!setCameraX)
            x = player.position.x;
        if(!setCameraY)
            y = player.position.y;

        transform.position = new Vector3(x, y, -10);
    }
}
