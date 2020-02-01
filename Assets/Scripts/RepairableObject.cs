using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairableObject : MonoBehaviour
{
    List<GameObject> placeableObjects = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
        Rigidbody[] rs = gameObject.GetComponentsInChildren<Rigidbody>();
        if (rs != null)
        {
            foreach (Rigidbody r in rs)
            {
                if (r != null && r.transform != null && r.transform.gameObject != null)
                    placeableObjects.Add(r.transform.gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
