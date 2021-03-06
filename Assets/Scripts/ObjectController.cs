﻿using System.Collections;
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
    public AudioSource moveSound;
    public AudioSource rotateSound;
    public AudioSource placedSound;


    //public List<RepairableObject> repairableObjects = new List<RepairableObject>();
    RepairableObject curRepairableObject;
    private Vector3 offset;
    private Vector3 velocity = Vector3.zero;

    public GameObject selectedObject;
    public GameObject floor;

    public ObjectMode selectedObjectMode = ObjectMode.None;
    private bool wasMoving = false;
    private Color savedColor;

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
            Rigidbody r = selectedObject.GetComponent<Rigidbody>();
            if (r)
            {
                r.velocity = Vector3.zero;
            }
            if (selectedObjectMode == ObjectMode.Moving)
            {
                Vector3 newPos = GetMouseAsWorldPoint() + offset;
                newPos[1] = Mathf.Clamp(newPos.y, floor.transform.position.y, 100.0f);
                selectedObject.transform.position = Vector3.SmoothDamp(selectedObject.transform.position, newPos, ref velocity, smoothTime);
            }
            if(selectedObjectMode == ObjectMode.Rotating)
            {
                float xInput = Input.GetAxis("Mouse X");
                float yInput = -Input.GetAxis("Mouse Y");
                if (Mathf.Abs(xInput) > Mathf.Abs(yInput))
                {
                    xRot = xInput * xSpeed * 0.02f;
                    zRot = 0.0f;
                }
                else
                {
                    zRot = yInput * zSpeed * 0.02f;
                    xRot = 0.0f;
                }

                selectedObject.transform.RotateAround(selectedObject.transform.position, Vector3.up, xRot);

                if (Vector3.Dot(selectedObject.transform.forward, Vector3.forward) > 0.0f)
                {
                    selectedObject.transform.RotateAround(selectedObject.transform.position, Vector3.forward, zRot);
                }
                else;
                {
                    selectedObject.transform.RotateAround(selectedObject.transform.position, Vector3.right, zRot);
                }
            }
        }
    }

    // Late Update is called once per frame, after all other Updates
    private void LateUpdate()
    {
        bool repairableObjectInitialized = false;
        
        if (curRepairableObject && curRepairableObject.isInitialized)
        {
            repairableObjectInitialized = true;
        }

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
                    //Things are only selectable once the repairable object is initialized
                    if (repairableObjectInitialized == false)
                    {
                        return;
                    }

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
                bool tryPlace = curRepairableObject.TryPlace(selectedObject);
                if (tryPlace)
                {
                    OnLockObject();
                }                

                if (!tryPlace)
                {
                    Collider col = selectedObject.GetComponent<Collider>();
                    if (col) col.enabled = true;
                }
            }
        }
    }

    public void SetCurrentRepairableObject(RepairableObject r)
    {
        curRepairableObject = r;
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
                selectedObject.GetComponent<MeshRenderer>().material.SetColor("_AmbientColor", Color.red);
                moveSound.Play();
                break;
            case (ObjectMode.Rotating):
                wasMoving = false;
                selectedObjectMode = ObjectMode.Rotating;
                selectedObject.GetComponent<MeshRenderer>().material.SetColor("_AmbientColor", Color.green);
                rotateSound.Play();
                break;
            case (ObjectMode.Stationary):
                selectedObjectMode = ObjectMode.Stationary;
                break;
            case (ObjectMode.None):
            default:
                selectedObject.GetComponent<MeshRenderer>().material.SetColor("_AmbientColor", savedColor);
                selectedObjectMode = ObjectMode.None;
                break;
        }
    }

    public void OnSelectObject(GameObject newObj)
    {
        if(curRepairableObject.piecesPlaced() == 0)
        {
            curRepairableObject.SetPlaced(newObj.transform);
            return;
        }

        selectedObject = newObj;
        savedColor = newObj.GetComponent<MeshRenderer>().material.GetColor("_AmbientColor");
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
        placedSound.Play();
    }
}
