using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float rotationSpeed = 0.1f;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Single touch for Y-axis rotation
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                // Rotate only around the Y axis
                float rotY = -touch.deltaPosition.x * rotationSpeed;
                transform.Rotate(0, rotY, 0, Space.World);
            }
        }
    }
}
