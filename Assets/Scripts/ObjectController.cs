using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public List<RepairableObject> repairableObjects = new List<RepairableObject>();
    private Vector3 offset;
    private Vector3 velocity = Vector3.zero;

    public GameObject selectedObject;
    public GameObject floor;

    private bool isObjectMoving = false;
    private float zCoord;
    private float smoothTime = 0.1f;

    // Update is called once per frame
    void Update()
    {
        if (isObjectMoving && selectedObject != null)
        {
            Vector3 newPos = GetMouseAsWorldPoint() + offset;
            newPos[1] = Mathf.Clamp(newPos.y, floor.transform.position.y, 100.0f);
            selectedObject.transform.position = Vector3.SmoothDamp(selectedObject.transform.position, newPos, ref velocity, smoothTime);
        }
    }

    // Late Update is called once per frame, after all other Updates
    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100.0f, LayerMask.GetMask("unhit", "floor")))
            {
                //Hit the Floor
                if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("floor"))
                {
                    Camera.main.GetComponent<CameraController>().OnSelectCamera();
                }
                //Hit an Object
                else
                {
                    if (selectedObject != null && selectedObject != hitInfo.collider.gameObject)
                    {
                        OnDeselectObject();
                    }

                    if (selectedObject == null)
                    {
                        OnSelectObject(hitInfo.collider.gameObject);
                    }

                    zCoord = Camera.main.WorldToScreenPoint(selectedObject.transform.position).z;
                    offset = selectedObject.transform.position - GetMouseAsWorldPoint();
                    isObjectMoving = true;
                }
                Camera.main.GetComponent<CameraController>().OnDeselectCamera();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Camera.main.GetComponent<CameraController>().OnDeselectCamera();
            isObjectMoving = false;
            if (selectedObject != null)
            {
                foreach (RepairableObject r in repairableObjects)
                {
                    bool tryPlace = r.TryPlace(selectedObject);
                    if (tryPlace)
                    {
                        OnLockObject();
                    }
                    else
                    {
                        selectedObject.GetComponent<Collider>().enabled = true;
                    }
                }
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

    public void OnSelectObject(GameObject newObj)
    {
        selectedObject = newObj;
        //selectedObject.layer = LayerMask.NameToLayer("hit");
        selectedObject.GetComponent<MeshRenderer>().material.color = Color.red;
        selectedObject.GetComponent<Rigidbody>().useGravity = false;
        selectedObject.GetComponent<Rigidbody>().freezeRotation = true;
        selectedObject.GetComponent<Collider>().enabled = false;
    }
    public void OnDeselectObject()
    {
        selectedObject.GetComponent<MeshRenderer>().material.color = Color.grey;
        selectedObject.GetComponent<Rigidbody>().useGravity = true;
        selectedObject.GetComponent<Rigidbody>().freezeRotation = false;
        selectedObject.GetComponent<Collider>().enabled = true;
        selectedObject = null;
    }

    public void OnLockObject()
    {
        selectedObject.layer = LayerMask.NameToLayer("hit");
        selectedObject.GetComponent<MeshRenderer>().material.color = Color.grey;
        selectedObject = null;
    }
}
