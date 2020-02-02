using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectMode
{ 
    Moving,
    Rotating,
    Stationary,
    None
}

public class ObjectController : MonoBehaviour
{
    public List<RepairableObject> repairableObjects = new List<RepairableObject>();
    private Vector3 offset;
    private Vector3 velocity = Vector3.zero;

    public GameObject selectedObject;
    public GameObject floor;

    public ObjectMode selectedObjectMode = ObjectMode.None;
    private bool wasMoving = false;

    private float zCoord;
    private float xRot = 0.0f;
    private float yRot = 0.0f;
    private float zRot = 0.0f;
    private float xSpeed = 150.0f;
    private float zSpeed = 150.0f;

    private float smoothTime = 0.1f;

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if( selectedObject != null )
        {
            if (selectedObjectMode == ObjectMode.Moving)
            {
                Vector3 newPos = GetMouseAsWorldPoint() + offset;
                newPos[1] = Mathf.Clamp(newPos.y, floor.transform.position.y, 100.0f);
                selectedObject.transform.position = Vector3.SmoothDamp(selectedObject.transform.position, newPos, ref velocity, smoothTime);
            }
            if(selectedObjectMode == ObjectMode.Rotating)
            { 
                xRot = Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                zRot = Input.GetAxis("Mouse Y") * zSpeed * 0.02f;

                selectedObject.transform.RotateAround(selectedObject.transform.position, Vector3.up, xRot);
                selectedObject.transform.RotateAround(selectedObject.transform.position, Vector3.right, zRot);
            }
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
                    Camera.main.GetComponent<CameraController>().OnDeselectCamera();
                    if (selectedObject != null)
                    {
                        if (selectedObject == hitInfo.collider.gameObject)
                        {
                            OnSwitchObjectMode(wasMoving ? ObjectMode.Rotating : ObjectMode.Moving);
                        }
                        else
                        {
                            OnDeselectObject();
                        }
                    }

                    if (selectedObject == null)
                    {
                        OnSelectObject(hitInfo.collider.gameObject);
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Camera.main.GetComponent<CameraController>().OnDeselectCamera();
            OnSwitchObjectMode(ObjectMode.Stationary);
            if (selectedObject != null)
            {
                bool tryPlace = false;
                foreach (RepairableObject r in repairableObjects)
                {
                    tryPlace = r.TryPlace(selectedObject);
                    if (tryPlace)
                    {
                        OnLockObject();
                        break;
                    }
                }

                if (!tryPlace)
                {
                    Collider col = selectedObject.GetComponent<Collider>();
                    if (col) col.enabled = true;
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

    public void OnSwitchObjectMode(ObjectMode newMode)
    {
        switch (newMode)
        {
            case (ObjectMode.Moving):
                wasMoving = true;
                selectedObjectMode = ObjectMode.Moving;
                zCoord = Camera.main.WorldToScreenPoint(selectedObject.transform.position).z;
                offset = selectedObject.transform.position - GetMouseAsWorldPoint();
                selectedObject.GetComponent<MeshRenderer>().material.color = Color.red;
                break;
            case (ObjectMode.Rotating):
                wasMoving = false;
                selectedObjectMode = ObjectMode.Rotating;
                selectedObject.GetComponent<MeshRenderer>().material.color = Color.green;
                break;
            case (ObjectMode.Stationary):
                selectedObjectMode = ObjectMode.Stationary;
                break;
            case (ObjectMode.None):
            default:
                selectedObject.GetComponent<MeshRenderer>().material.color = Color.white;
                selectedObjectMode = ObjectMode.None;
                break;
        }
    }

    public void OnSelectObject(GameObject newObj)
    {
        selectedObject = newObj;
        OnSwitchObjectMode(ObjectMode.Moving);

        Rigidbody r = selectedObject.GetComponent<Rigidbody>();
        if (r)
        {
            r.velocity = Vector3.zero;
            r.useGravity = false;
            r.freezeRotation = true;
        }
        Collider c = selectedObject.GetComponent<Collider>();
        if (c)
        {
            c.enabled = false;
        }
    }

    public void OnDeselectObject()
    {
        OnSwitchObjectMode(ObjectMode.None);
        Rigidbody r = selectedObject.GetComponent<Rigidbody>();
        if (r)
        {
            r.useGravity = true;
            r.freezeRotation = false;
        }
        Collider c = selectedObject.GetComponent<Collider>();
        if (c)
        {
            c.enabled = true;
        }

        selectedObject = null;
    }

    public void OnLockObject()
    {
        OnSwitchObjectMode(ObjectMode.None);
        selectedObject = null;
    }
}
