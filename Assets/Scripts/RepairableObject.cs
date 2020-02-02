using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class RepairableObject : MonoBehaviour
{
    private static float SNAP_DISTANCE = 5f;
    private static float SNAP_ROTATION = 180f;

    public RepairableObjectManager manager;

    List<Transform> placeableObjects = new List<Transform>();
    Dictionary<Transform, bool> placeableObjectCanBePlaced = new Dictionary<Transform, bool>();
    Dictionary<Transform, Vector3> placeableObjectPos = new Dictionary<Transform, Vector3>();
    Dictionary<Transform, Quaternion> placeableObjectRot = new Dictionary<Transform, Quaternion>();

    Dictionary<Transform, List<Transform>> neighbors = new Dictionary<Transform, List<Transform>>();

    int initialPlacedIndex;

    int numPlaced = 0;
    public bool isInitialized = false;
    public bool finished = false;

    public ParticleSystem completeParticles;

    // Start is called before the first frame update
    void Start()
    {
        isInitialized = false;
        finished = false;
        Rigidbody[] rs = gameObject.GetComponentsInChildren<Rigidbody>();
        //Debug.Log("STARTING OBJECT:  " + gameObject.name +"  rigidbodies:  "+ rs.Length);
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
                    HandleMouseDown m = r.gameObject.AddComponent<HandleMouseDown>();
                    m.target = this;

                    r.isKinematic = true;
                    //r.AddExplosionForce(300f, transform.position, 5, 3.0F);
                }
            }
        }

        float originalY = transform.position.y;

        transform.position = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);

        transform.DOLocalMoveY(originalY, 3);
    }

    public void HandleClicked()
    {
        Debug.Log("MOUSE DOWN!");
        if (isInitialized == false)
        {
            StartObject();
            isInitialized = true;
        }        
    }

    void StartObject()
    { 

        foreach (Transform t in placeableObjects)
        {
            Rigidbody r = t.gameObject.GetComponent<Rigidbody>();
            r.isKinematic = false;
            r.AddExplosionForce(300f, transform.position, 5, 3.0F);
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
        t.DORotateQuaternion(placeableObjectRot[t],.5f).OnComplete(CheckFinished);        

        Rigidbody r = t.GetComponent<Rigidbody>();
        if (r)
        {
            r.isKinematic = true;
        }

        numPlaced++;
        if(numPlaced >= placeableObjects.Count)
        {
            Debug.Log("YOU WIN!");
            finished = true;            
        }
    }

    void CheckFinished()
    {
        if (finished) { 
            if(completeParticles)
            {
                completeParticles.Play();
            }
            transform.DOShakePosition(3, .1f);
            transform.DOLocalMoveY(transform.position.y + 3.5f, 3).OnComplete(StartNextObject);
        }
    }

    void StartNextObject()
    {
        if (manager)
            manager.NextRepairableObject();
    }
}
