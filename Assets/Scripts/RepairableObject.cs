using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RepairableObject : MonoBehaviour
{
    List<Transform> placeableObjects = new List<Transform>();
    Dictionary<Transform, bool> placeableObjectCanBePlaced = new Dictionary<Transform, bool>();
    Dictionary<Transform, Vector3> placeableObjectPos = new Dictionary<Transform, Vector3>();
    Dictionary<Transform, Quaternion> placeableObjectRot = new Dictionary<Transform, Quaternion>();

    Dictionary<Transform, List<Transform>> neighbors = new Dictionary<Transform, List<Transform>>();

    int initialPlacedIndex;
    // Start is called before the first frame update
    void Start()
    {
        
        Rigidbody[] rs = gameObject.GetComponentsInChildren<Rigidbody>();
        if (rs != null)
        {
            foreach (Rigidbody r in rs)
            {
                if (r != null && r.transform != null && r.transform.gameObject != null)
                {
                    placeableObjects.Add(r.transform);
                    placeableObjectCanBePlaced.Add(r.transform, false);
                    placeableObjectPos.Add(r.transform, r.transform.position);
                    placeableObjectRot.Add(r.transform, r.transform.rotation);
                }                
            }
        }

        initialPlacedIndex = Random.Range(0, placeableObjects.Count);
        SetPlaced(placeableObjects[initialPlacedIndex]);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void SetPlaced(Transform t)
    {
        t.position = placeableObjectPos[t];
        t.rotation = placeableObjectRot[t];
        Rigidbody r = t.GetComponent<Rigidbody>();
        if (r)
        {
            r.isKinematic = true;
        }
    }
}
