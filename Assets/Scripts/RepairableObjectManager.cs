using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class RepairableObjectManager : MonoBehaviour
{
    public Image fadeImage;
    public Image fadeLogo;
    public Text fadeText;
    public ObjectController objectConroller;
    public List<RepairableObject> repairableObjects;
    public int currentObjectIndex = 0;

    public List<GameObject> ascendableObjects;

    // Start is called before the first frame update
    void Start()
    {
        foreach (RepairableObject r in repairableObjects)
        {
            r.manager = this;
            r.gameObject.SetActive(false);
        }
        fadeImage.color = new Color(1, 1, 1, 1);
        fadeLogo.color = new Color(1, 1, 1, 0);
        //fadeImage.DOFade(0, 3).SetDelay(2).OnComplete(StartGame);
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(fadeLogo.DOFade(1,1).SetDelay(1));
        mySequence.Append(fadeLogo.DOFade(0, 1).SetDelay(2).OnComplete(StartGame));
        mySequence.Append(fadeImage.DOFade(0, 3));
        //fadeImage.DOFade(0, 3).SetDelay(2).OnComplete(StartGame);
    }

    public void StartGame()
    {
        Camera.main.GetComponent<CameraController>().StartGame();
        repairableObjects[currentObjectIndex].gameObject.SetActive(true);
        objectConroller.SetCurrentRepairableObject(repairableObjects[currentObjectIndex]);
    }

    public IEnumerator DeactivateAfterTwoSeconds(GameObject go)
    {
        yield return new WaitForSeconds(2);
        go.SetActive(false);
    }

    public void NextRepairableObject()
    {
        //repairableObjects[currentObjectIndex].gameObject.SetActive(false);
        StartCoroutine(DeactivateAfterTwoSeconds(repairableObjects[currentObjectIndex].gameObject));        
        currentObjectIndex++;
        if (currentObjectIndex < repairableObjects.Count)
        {
            repairableObjects[currentObjectIndex].gameObject.SetActive(true);
            objectConroller.SetCurrentRepairableObject(repairableObjects[currentObjectIndex]);
        }
        else
        {
            StartCoroutine(EndSequence());
            //TODO what to do when they finish all objects?


            /*objectConroller.gameObject.SetActive(false);
            Camera.main.GetComponent<CameraController>().enabled = false;

            foreach (GameObject g in ascendableObjects)
            {
                g.transform.DOMove(new Vector3(g.transform.position.x + 2 * Random.value, g.transform.position.y + 10 + 2 * Random.value, g.transform.position.z + 2 * Random.value), 10);
                g.transform.DORotate(new Vector3(g.transform.rotation.eulerAngles.x + 30 * Random.value, g.transform.rotation.eulerAngles.y + 30f * Random.value, g.transform.rotation.eulerAngles.z + 30f * Random.value), 10);
            }
            Transform c = Camera.current.transform;
            c.DOMove(new Vector3(c.position.x, c.position.y +40, c.position.z), 10);
            //c.DORotate(new Vector3(90, 0,0), 10);
            c.DOLookAt(new Vector3(c.position.x, 0, c.position.z), 5);
            fadeImage.DOFade(1, 10);*/
        }
    }

    public IEnumerator EndSequence()
    {
        yield return new WaitForSeconds(2);

        objectConroller.gameObject.SetActive(false);
        Camera.main.GetComponent<CameraController>().enabled = false;

        foreach (GameObject g in ascendableObjects)
        {
            g.transform.DOMove(new Vector3(g.transform.position.x + 2 * Random.value, g.transform.position.y + 10 + 2 * Random.value, g.transform.position.z + 2 * Random.value), 10);
            g.transform.DORotate(new Vector3(g.transform.rotation.eulerAngles.x + 30 * Random.value, g.transform.rotation.eulerAngles.y + 30f * Random.value, g.transform.rotation.eulerAngles.z + 30f * Random.value), 10);
        }
        Transform c = Camera.current.transform;
        c.DOMove(new Vector3(c.position.x, c.position.y + 40, c.position.z), 10);
        //c.DORotate(new Vector3(90, 0,0), 10);
        c.DOLookAt(new Vector3(c.position.x, 0, c.position.z), 5);
        
        fadeImage.DOFade(1, 10);
        fadeLogo.DOFade(1, 2).SetDelay(7f);
        fadeText.DOFade(1, 2).SetDelay(8f); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
