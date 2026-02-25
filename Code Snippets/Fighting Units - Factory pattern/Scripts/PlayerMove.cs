using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float sensitivity = 50F;
    public float maxFov = 12F;
    public float minFov = 1F;

    float size;
    float horizontalMovement = 0;
    public float movementSpeed = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    { 
        //movement
        float lateralMovement = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
        
        float verticalMovement = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;

        transform.Translate(lateralMovement, verticalMovement * 1.5f, horizontalMovement);

        
        size = Camera.main.orthographicSize;
        size += Input.GetAxis("Mouse ScrollWheel") * sensitivity * Time.deltaTime;
        size = Mathf.Clamp(size, minFov, maxFov);
        Camera.main.orthographicSize = size;
        
    }
}
