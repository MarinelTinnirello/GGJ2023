using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    [Header("Movement")]
    public float moveSpd;
    public float jumpForce;
    public Vector2 moveInput;
    private Rigidbody rb;

    [Header("Ground Check")]
    public LayerMask groundMask;
    public Transform groundPoint;
    private bool isGround;
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector3(moveInput.x * moveSpd, rb.velocity.y, moveInput.y * moveSpd);

        RaycastHit hit;

        if (Physics.Raycast(groundPoint.position, Vector3.down, out hit, .3f, groundMask))
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }

        // add isGround, it got screwy
        if (Input.GetButtonDown("Jump"))
        {
            rb.velocity += new Vector3(0f, jumpForce, 0f);
        }
    }
}
