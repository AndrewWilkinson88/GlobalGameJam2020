using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    private Vector3 offset;
    private Vector3 velocity = Vector3.zero;

    public GameObject selectedObject;
    public GameObject floor;

    private bool isMoving = false;
    private float zCoord;
    private float smoothTime = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100.0f, LayerMask.GetMask("unhit", "floor")))
            {
                //Hit the Floor
                if( hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("floor"))
                {
                    Camera.main.GetComponent<CameraController>().OnSelectCamera();
                }
                else
                {
                    Camera.main.GetComponent<CameraController>().OnDeselectCamera();

                    if (selectedObject != null && selectedObject != hitInfo.collider.gameObject)
                    {
                        selectedObject.GetComponent<MeshRenderer>().material.color = Color.grey;
                        selectedObject.GetComponent<Rigidbody>().useGravity = true;
                        selectedObject.GetComponent<Rigidbody>().freezeRotation = false;
                        selectedObject.GetComponent<Collider>().enabled = true;
                        selectedObject = null;
                    }
                    
                    if( selectedObject == null)
                    {
                        selectedObject = hitInfo.collider.gameObject;
                        //selectedObject.layer = LayerMask.NameToLayer("hit");
                        selectedObject.GetComponent<MeshRenderer>().material.color = Color.red;
                        selectedObject.GetComponent<Rigidbody>().useGravity = false;
                        selectedObject.GetComponent<Rigidbody>().freezeRotation = true;
                        selectedObject.GetComponent<Collider>().enabled = false;
                    }

                    zCoord = Camera.main.WorldToScreenPoint(selectedObject.transform.position).z;
                    offset = selectedObject.transform.position - GetMouseAsWorldPoint();
                    isMoving = true;
                }
            }
        }

        if (isMoving && selectedObject != null)
        {
            Vector3 newPos = GetMouseAsWorldPoint() + offset;
            newPos[1] = Mathf.Clamp(newPos.y, floor.transform.position.y, 100.0f);
            selectedObject.transform.position = Vector3.SmoothDamp(selectedObject.transform.position, newPos, ref velocity, smoothTime);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Camera.main.GetComponent<CameraController>().OnDeselectCamera();
            isMoving = false;
            if (selectedObject != null)
            {
                selectedObject.GetComponent<Collider>().enabled = true;
            }
        }
    }
    
    private Vector3 GetMouseAsWorldPoint()
    {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;

        // z coordinate of game object on screen
        mousePoint.z = zCoord;

        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
