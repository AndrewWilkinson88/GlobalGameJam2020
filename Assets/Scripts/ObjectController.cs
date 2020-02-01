using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    private Vector3 offset;
    private float zCoord;

    public GameObject selectedObject;

    float x = 0.0f;
    float y = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedObject == null && Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100.0f, LayerMask.GetMask("unhit")))
            {
                selectedObject = hitInfo.collider.gameObject;
                //selectedObject.layer = LayerMask.NameToLayer("hit");
                selectedObject.GetComponent<MeshRenderer>().material.color = Color.red;
                selectedObject.GetComponent<Rigidbody>().useGravity = false;
                selectedObject.GetComponent<Collider>().enabled = false;

                zCoord = Camera.main.WorldToScreenPoint(selectedObject.transform.position).z;
                offset = selectedObject.transform.position - GetMouseAsWorldPoint();
                x = 0.0f;
                y = 0.0f;
            }
        }

        if ( selectedObject != null )
        {
            selectedObject.transform.position = GetMouseAsWorldPoint() + offset;
            if( Input.GetMouseButtonUp(0))
            {
                selectedObject.GetComponent<MeshRenderer>().material.color = Color.grey;
                selectedObject.GetComponent<Rigidbody>().useGravity = true;
                selectedObject.GetComponent<Collider>().enabled = true;
                selectedObject = null;
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
