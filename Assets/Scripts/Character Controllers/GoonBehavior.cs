using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoonBehavior : MonoBehaviour
{
    Rigidbody rb;
    DJBehavior dj;
    GameObject computer;

    static Queue<GoonBehavior> waitingGoons = new Queue<GoonBehavior>();
    GoonBehavior activeGoon;

    float jumpLine;
    private enum State { onFloor, onStage, dragging }
    State state = State.onFloor;
    Vector3 moveDirection;
    public float speed = 3f;
    Vector3 jumpAngle = new Vector3(0f, 1f, 0.5f);
    public float jumpForce = 6f;
    bool hasJumped = false;

    Vector3 relationshipToActiveGoon;
    bool isReadyToGrab = false;

    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpLine = GameObject.Find("Jump Line").transform.position.z;
        dj = GameObject.FindGameObjectWithTag("DJ").GetComponent<DJBehavior>();
        computer = GameObject.FindGameObjectWithTag("Computer");
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.onFloor)
        {
            UpdateOnFloor();
        }
        else if (state == State.onStage)
        {
            UpdateOnStage();
        }
        else if (state == State.dragging)
        {
            UpdateDragging();
        }
    }

    void UpdateOnFloor()
    {
        if (transform.position.z < jumpLine)
        {
            rb.velocity = Vector3.forward * speed;
        }
        else if (!hasJumped)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(jumpAngle * jumpForce, ForceMode.Impulse);
            hasJumped = true;
        }
    }

    void UpdateOnStage()
    {
        if (isReadyToGrab)
        {
            transform.position = activeGoon.transform.position + relationshipToActiveGoon;
            return;
        }
        // TODO: replace this with queue deciding who will grab next ***********************************
        if (isReadyToGrab && !activeGoon)
        {
            StartDragging();
        }
        
        Vector3 djLocation = dj.transform.position;
        moveDirection = (djLocation - transform.position).normalized;

        rb.velocity = moveDirection * speed;
    }

    void UpdateDragging()
    {
        rb.velocity = moveDirection * (speed / 2f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // When they jump onto the stage
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && state != State.onStage && hasJumped)
        {
            state = State.onStage;
        }

        // When they get to the DJ
        if (collision.gameObject.CompareTag("DJ") && state != State.dragging)
        {
            if (dj.isBeingDragged)
            {
                GetReadyToGrab();
                return;
            }

            StartDragging();
        }

        // TODO: replace this with "when dead" *********************************************************
        // When the player touches them
        if (collision.gameObject.CompareTag("Player"))
        {
            if (state == State.dragging)
            {
                dj.StopDragging();
            }
            if (waitingGoons.Count > 0)
            {
                PickNextGoon();
            }
            Destroy(this.gameObject);
        }
    }

    private void StartDragging()
    {
        dj.StartDragging(this);
        state = State.dragging;
        Vector3 computerLocation = computer.transform.position;
        moveDirection = -(computerLocation - transform.position).normalized;
        if (Mathf.Abs(moveDirection.x) < .25f)
        {
            if (Random.Range(0, 1) == 0)
            {
                moveDirection.x = Random.Range(.2f, .7f);
            }
            else
            {
                moveDirection.x = Random.Range(-.2f, -.7f);
            }
        }
        moveDirection.y = 0f;
        if (moveDirection.z < 0f)
        {
            moveDirection.z = Random.Range(0f, .25f);
        }
        moveDirection = moveDirection.normalized;
    }

    void GetReadyToGrab()
    {
        waitingGoons.Enqueue(this);
        activeGoon = dj.ActiveGoon;
        relationshipToActiveGoon = transform.position - activeGoon.transform.position;
        isReadyToGrab = true;
    }

    static void PickNextGoon()
    {
        GoonBehavior nextGoon = waitingGoons.Dequeue();
        nextGoon.StartDragging();
        foreach (GoonBehavior goon in waitingGoons)
        {
            goon.activeGoon = nextGoon;
            goon.relationshipToActiveGoon = goon.transform.position - nextGoon.transform.position;
        }
    }
}
