using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]

public class NearBy : MonoBehaviour
{
    public Collider triggerCollider;

    // Start is called before the first frame update
    void Start()
    {
        if (!triggerCollider) triggerCollider = gameObject.GetComponent<Collider>();
        if (triggerCollider) triggerCollider.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
