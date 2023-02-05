using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagpoleEncircler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Sin(Time.frameCount/300f)*10f, 0, Mathf.Cos(Time.frameCount/300f)*10f);
    }
}
