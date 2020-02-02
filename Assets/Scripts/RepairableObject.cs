using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class RepairableObject : MonoBehaviour
{
    private static float SNAP_DISTANCE = 1f;
    private static float SNAP_ROTATION = 60f;

    List<Transform> placeableObjects = new List<Transform>();
    Dictionary<Transform, bool> placeableObjectCanBePlaced = new Dictionary<Transform, bool>();
    Dictionary<Transform, Vector3> placeableObjectPos = new Dictionary<Transform, Vector3>();
    Dictionary<Transform, Quaternion> placeableObjectRot = new Dictionary<Transform, Quaternion>();

    Dictionary<Transform, List<Transform>> neighbors = new Dictionary<Transform, List<Transform>>();

    int initialPlacedIndex;

    int numPlaced = 0;

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

                    r.gameObject.layer = LayerMask.NameToLayer("unhit");
                    r.gameObject.AddComponent<KeepInPlay>();

                    r.AddExplosionForce(300f, transform.position, 5, 3.0F);
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

    public bool TryPlace(GameObject g)
    {
        Debug.Log("TRYING PLACE");
        //TODO for now let pieces be placed even if not attached to anything
        if (placeableObjectCanBePlaced.ContainsKey(g.transform) /* && placeableObjectCanBePlaced[g.transform] */)
        {
            float dist = Vector3.Distance(g.transform.position, placeableObjectPos[g.transform]);
            if (dist < SNAP_DISTANCE)
            {
                float angle = Quaternion.Angle(g.transform.rotation, placeableObjectRot[g.transform]);
                if (angle < SNAP_ROTATION)
                {
                    SetPlaced(g.transform);
                    //Debug.Log("PIECE PLACED");
                    return true;
                }
                else {
                    Debug.Log("PIECE NOT ROTATED CORRECTLY.  ANGLE: " + angle + "  snap rot: " + SNAP_ROTATION);
                    return false; 
                }
            } else {
                Debug.Log("PIECE NOT CLOSE.  dist: " + dist + "  snap dist: " + SNAP_DISTANCE);
                return false; 
            }
        }
        else {
            Debug.Log("PIECE NOT PLACABLE");
            return false; 
        }
    }

    void SetPlaced(Transform t)
    {
        t.gameObject.layer = LayerMask.NameToLayer("hit");
        t.DOMove(placeableObjectPos[t], .5f);
        t.DORotate(placeableObjectRot[t].eulerAngles, .5f);

        Rigidbody r = t.GetComponent<Rigidbody>();
        if (r)
        {
            r.isKinematic = true;
        }

        numPlaced++;
        if(numPlaced >= placeableObjects.Count)
        {
            Debug.Log("YOU WIN!");
        }
    }
}
