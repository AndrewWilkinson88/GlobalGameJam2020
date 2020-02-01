using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject currentTarget;

    float xSpeed = 120.0f;
    float ySpeed = 120.0f;
    float distance;

    float x = 0.0f;
    float y = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        transform.LookAt(currentTarget.transform);

        x += Input.GetAxis("Mouse X") * xSpeed * distance* 0.02f;
        y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 position = rotation * new Vector3(0.0f,0.0f,-distance) + currentTarget.transform.position;

        transform.rotation = rotation;
        transform.position = position;
    }

}
