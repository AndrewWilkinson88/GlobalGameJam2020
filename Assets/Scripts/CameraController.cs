﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public GameObject currentTarget;
    public GameObject Floor;

    Vector3 targetPoint;
    float yMinLimit = 5.0f;
    float yMaxLimit = 15.0f;

    float distanceMin = .5f;
    float distanceMax = 15f;

    float xSpeed = 100.0f;
    float ySpeed = 100.0f;
    float distance;

    float x = 0.0f;
    float y = 0.0f;
    float floorDist;
    bool isSelected = false;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        //Try to find a balance point between Floor and Object Breaking
        floorDist = Mathf.Abs(currentTarget.transform.position.y) + Mathf.Abs(Floor.transform.position.y);
        targetPoint = currentTarget.transform.position - Vector3.up * floorDist / 4.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelected)
        {
            distance = Vector3.Distance(transform.position, targetPoint);
            transform.LookAt(targetPoint);

            x += Input.GetAxis("Mouse X") * xSpeed * distance* 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 position = rotation * new Vector3(0.0f,0.0f,-distance) + targetPoint;

            transform.rotation = rotation;
            transform.position = position;
        }
    }

    public void OnSelectCamera()
    {
        isSelected = true;
    }

    public void OnDeselectCamera()
    {
        isSelected = false;
    }
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
