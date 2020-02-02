using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairableObjectManager : MonoBehaviour
{
    public ObjectController objectConroller;
    public List<RepairableObject> repairableObjects;
    public int currentObjectIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach (RepairableObject r in repairableObjects)
        {
            r.manager = this;
            r.gameObject.SetActive(false);
        }

        repairableObjects[currentObjectIndex].gameObject.SetActive(true);
        objectConroller.SetCurrentRepairableObject(repairableObjects[currentObjectIndex]);

    }

    public void NextRepairableObject()
    {
        repairableObjects[currentObjectIndex].gameObject.SetActive(false);
        currentObjectIndex++;
        if (currentObjectIndex < repairableObjects.Count)
        {
            repairableObjects[currentObjectIndex].gameObject.SetActive(true);
            objectConroller.SetCurrentRepairableObject(repairableObjects[currentObjectIndex]);
        }
        else
        {
            //TODO what to do when they finish all objects?
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
