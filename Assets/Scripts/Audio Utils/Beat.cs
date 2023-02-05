using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beat : MonoBehaviour
{
    BeatTracker tracker;

    public float speed = 0.2f;


    // Start is called before the first frame update
    void Start()
    {
        tracker = GameObject.Find("Beat Tracker").GetComponent<BeatTracker>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.left * speed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BeatTracker"))
        {
            if (tracker.isMusicPlaying)
            {
                
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("BeatTracker"))
        {
            if (tracker.isMusicPlaying)
            {
                
            }
            Destroy(this.gameObject);
        }
    }
}
