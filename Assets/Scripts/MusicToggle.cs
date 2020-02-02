using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicToggle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        Debug.Log("MUSIC MOUSE DOWN");
        AudioSource a = gameObject.GetComponent<AudioSource>();
        if (a)
        {
            if (a.isPlaying)
            {
                Debug.Log("STOP MUSIC");
                a.Stop();
            }
            else
            {
                Debug.Log("START MUSIC");
                a.Play();
            }
        }
    
    }
}
