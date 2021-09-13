using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField]
    public float speed = 1;

    Vector2 target = new Vector2(0, -0.07f);

    void Below()
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed);
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        Below();
    }
}
