using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : DamageDealer
{
    public float speed = 15f;
    public float hitOffset = 0f;
    public bool UseFirePointRotation;

    public Vector3 rotationOffset = new Vector3(0, 0, 0);

    public GameObject hit;
    public GameObject flash;
    public GameObject[] Detached;

    private Rigidbody rb;
    
    public override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody>();

        if (flash != null)
        {
            GameObject flashInstance = Instantiate(flash, transform.position, Quaternion.identity);
            flashInstance.transform.forward = gameObject.transform.forward;

            ParticleSystem flashPs = flashInstance.GetComponent<ParticleSystem>();

            if (flashPs != null)
            {
                Destroy(flashInstance, flashPs.main.duration);
            }
            else
            {
                ParticleSystem flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(flashInstance, flashPsParts.main.duration);
            }
        }
        Destroy(gameObject,5);
	}

    private void FixedUpdate ()
    {
		if (speed != 0)rb.velocity = transform.forward * speed;
	}

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        rb.constraints = RigidbodyConstraints.FreezeAll;
        speed = 0;

        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point + contact.normal * hitOffset;

        if (hit != null)
        {
            GameObject hitInstance = Instantiate(hit, pos, rot);

            if (UseFirePointRotation) { hitInstance.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0); }
            else if (rotationOffset != Vector3.zero) { hitInstance.transform.rotation = Quaternion.Euler(rotationOffset); }
            else { hitInstance.transform.LookAt(contact.point + contact.normal); }

            ParticleSystem hitPs = hitInstance.GetComponent<ParticleSystem>();

            if (hitPs != null)
            {
                Destroy(hitInstance, hitPs.main.duration);
            }
            else
            {
                ParticleSystem hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitInstance, hitPsParts.main.duration);
            }
        }
        foreach (var detachedPrefab in Detached)
        {
            if (detachedPrefab != null)
            {
                detachedPrefab.transform.parent = null;
            }
        }
        Destroy(gameObject);
    }
}
