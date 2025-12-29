using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private Rigidbody2D rb;
    [Header("Physics")]
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float maxNormalSpeed = 5f;
    [SerializeField] private float sprintSpeed = 15;
    [SerializeField] private float maxSprintSpeed = 8f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float frictionRatio = .9f;
    [Header("Ground Check")]
    [SerializeField] private float raycastOffsetX = .25f;
    [SerializeField] private float raycastOffsetY = .5f;
    [SerializeField] private float raycastLength = .15f;

    public enum PlayerState
    {
        Idle,
        Walking,
        Sprinting

    }
    private PlayerState playerState = PlayerState.Idle;
    private Vector2 velocity;
    private bool grounded = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        Vector2 inputVel = Vector2.zero;
        grounded = GroundCheck();

        if (Input.GetKey(KeyCode.RightArrow))
        {
            inputVel.x += movementSpeed;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            inputVel.x -= movementSpeed;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            SetPlayerState(PlayerState.Sprinting);
            inputVel.x *= sprintSpeed/movementSpeed;
        }
        else
        {
            SetPlayerState(PlayerState.Walking);
        }

        velocity = rb.velocity;
        velocity.x = inputVel.x;
        
        if(inputVel.magnitude == 0)
        {
            velocity.x *= frictionRatio;
        }

        if (grounded)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                velocity.y = jumpSpeed;
            }
        }

        rb.velocity = velocity;
        if (velocity.magnitude == 0)
        {
            SetPlayerState(PlayerState.Idle);
        }
    }

    bool GroundCheck()
    {
        for(int i = 0; i < 3; i++)
        {
            float xOffset = -raycastOffsetX + raycastOffsetX * i;
            Vector3 originPosition = transform.position + new Vector3(xOffset, -raycastOffsetY, 0);
            RaycastHit2D raycastHit2D = Physics2D.Raycast(originPosition, Vector2.down, raycastLength);
            // Debug.DrawLine(originPosition, originPosition + Vector3.down * raycastLength, Color.yellow);
            if(raycastHit2D.collider != null && raycastHit2D.collider.CompareTag("Ground"))
            {
                return true;
            }
        }

        return false;
    }

    void SetPlayerState(PlayerState pState)
    {
        playerState = pState;

        switch (playerState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Walking:
                break;
            case PlayerState.Sprinting:
                break;
        }
    }
}
