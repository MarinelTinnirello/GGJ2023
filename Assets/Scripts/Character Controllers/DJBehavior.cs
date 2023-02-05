using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DJBehavior : MonoBehaviour
{
    public BeatTracker tracker;
    Rigidbody rb;
    public GoonBehavior ActiveGoon { get; set; }

    Vector3 originPoint;
    Vector3 distanceToStart;
    Vector3 directionToStart;
    [SerializeField] bool isAway = false;
    public bool isBeingDragged = false;
    Vector3 relationshipToGoon;
    public float speed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        tracker = GameObject.Find("Beat Tracker").GetComponent<BeatTracker>();
        rb = GetComponent<Rigidbody>();
        originPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        CheckState();
        if (isBeingDragged)
        {
            transform.position = ActiveGoon.transform.position + relationshipToGoon;
        }
        if (isAway && !isBeingDragged)
        {
            Move();
        }
    }

    void CheckState()
    {
        distanceToStart = originPoint - transform.position;

        if (isAway && distanceToStart.magnitude < 0.1f && !isBeingDragged)
        {
            rb.velocity = Vector3.zero;
            tracker.StartMusic();
            isAway = false;
        }
    }

    void Move()
    {
        directionToStart = distanceToStart.normalized;

        rb.velocity = directionToStart * speed;
    }

    public void StartDragging(GoonBehavior goon)
    {
        if (isBeingDragged) return;

        Debug.Log("Being dragged");

        if (!isAway)
        {
            tracker.StopMusic();
            isAway = true;
        }
        isBeingDragged = true;
        ActiveGoon = goon;
        relationshipToGoon = transform.position - goon.transform.position;
    }

    public void StopDragging()
    {
        ActiveGoon = null;
        isBeingDragged = false;
    }
}
