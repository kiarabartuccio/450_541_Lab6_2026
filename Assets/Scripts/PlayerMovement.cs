using UnityEngine;

// Moves the player with a Rigidbody: 
// A/D or Left/Right arrows to ROTATE, W/S or Up/Down to move Forward/Backward, Space to jump.
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    // How powerful the second jump is:
    [SerializeField] private float doubleJumpForce = 4f;
    [SerializeField] private float moveSpeed = 7f;      // units per second
    [SerializeField] private float turnSpeed = 150f;    // degrees per second
    [SerializeField] private float jumpForce = 5f;      // strength of the jump

    [Header("Ground Check Settings")]
    [SerializeField] private float groundDistance = 0.5f; // ray length
    [SerializeField] private LayerMask groundMask;        // ground layers

    private Rigidbody rb;               // the physics body we move
    private Vector3 moveDirection;      // worked out from input in Update()
    private float turnInput;            // horizontal input for rotation
    private bool isGrounded;            // true when standing on the ground
    private bool jumpRequested = false; // set in Update(), used later
    // FixedUpdate() will see this and perform the actual physics jump.
    private bool doubleJumpRequested = false;
    // Remembers whether we already used our one double jump
    // false = still available
    // true = already used
    private bool hasDoubleJumped = false;

    public bool IsGrounded => isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Update()
    {
        // 1. Ground Check
        isGrounded = Physics.Raycast(transform.position + transform.up*groundDistance/2, -transform.up, groundDistance, groundMask);

        // If we are touching the ground again, allow the player to double jump again on their next jump.
        // Back on the ground: the double jump is available again
        if (isGrounded)
        {
            hasDoubleJumped = false;
        }

        // 2. Read Inputs
        turnInput = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right turn input
        float moveZ = Input.GetAxisRaw("Vertical");  // W/S or Up/Down forward/backward

        // Rotate the player transform in Update for responsive turning visuals
        transform.Rotate(0f, turnInput * turnSpeed * Time.deltaTime, 0f);

        // Calculate forward movement relative to current facing direction
        moveDirection = transform.forward * moveZ;

        // 3. Handle Jump Input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpRequested = true;
        }
    }

private void FixedUpdate()
    {
        MovePlayer();
 
        if (jumpRequested)
        {
            Jump(jumpForce);
            jumpRequested = false;
        }
 
        // NEW: the second jump, in mid-air
        if (doubleJumpRequested)
        {
            Jump(doubleJumpForce);
            doubleJumpRequested = false;
        }
    }
 
    private void Jump(float force)
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f,
                                        rb.linearVelocity.z);
        rb.AddForce(Vector3.up * force, ForceMode.VelocityChange);
    }


    private void MovePlayer()
    {
        // Calculate velocity based on current forward vector
        Vector3 targetVelocity = moveDirection * moveSpeed;

        // Apply movement while preserving vertical velocity (gravity/jumping)
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }

    // private void Jump()
    // {
    //     rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
    //     rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    // }

        // Tries to jump a second time in mid-air.
    // Returns true if the double jump really happened.
    public bool TryDoubleJump()
    {
        // Not allowed on the ground, or if it was already used
        if (isGrounded || hasDoubleJumped) return false;
 
        doubleJumpRequested = true;
        hasDoubleJumped = true;
        return true;
    }

}