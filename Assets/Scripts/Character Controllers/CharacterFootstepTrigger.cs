using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFootstepTrigger : MonoBehaviour
{
    [HideInInspector]
    public CharacterFootstepSystem footstepSystem;

    [Range(0f,1f)]
    public float triggerRadius = 0.1f;
    private float lastTriggerRadius;

    public LayerMask groundCheckLayers;

    [HideInInspector]
    public bool isGrounded = true;
    private bool footIsDown;

    private SphereCollider col;

    void Start()
    {
        col = GetComponent<SphereCollider>();
    }

    void Update()
    {
        UpdateCollider();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((groundCheckLayers.value & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            if (footIsDown || !isGrounded) return;
            footstepSystem?.CheckFootStepType(transform.position);
            footIsDown = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if ((groundCheckLayers.value & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            if (!footIsDown) return;
            footIsDown = false;
        }
    }

    private void UpdateCollider()
    {
        if (lastTriggerRadius != triggerRadius)
        {
            if(col)col.radius = triggerRadius;
            lastTriggerRadius = triggerRadius;
        }
    }
}