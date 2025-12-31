using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    private Rigidbody2D rb;
    [Header("Physics")]
    [SerializeField] private float maxNormalSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float horizToVertVel;
    [SerializeField] private float airborneAdj;
    [SerializeField] private float timeToFullAcceleration;

    [Header("Ground Check")]
    [SerializeField] private float raycastOffsetX;
    [SerializeField] private float raycastOffsetY;
    [SerializeField] private float raycastLength;

    private float maxSprintSpeed { get { return 2 * maxNormalSpeed; } }

    
    float movingTime = 0;
    float normalizedTime = 0;
    float maxVelocity;

    private Animator animator;
    private SpriteRenderer spriteRenderer;


    public enum PlayerState
    {
        Idle,
        Walking,
        Sprinting,
        Jumping,
        Climbing

    }
    private PlayerState playerState = PlayerState.Idle;
    PlayerState prevState = PlayerState.Idle;

    private Vector2 inputVel = Vector2.zero;
    private Vector2 velocity;
    private bool grounded = false;

    // Start is called before the first frame update

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        maxVelocity = maxNormalSpeed;
    }

    void FixedUpdate()
    {
        grounded = GroundCheck();
        velocity = rb.velocity;

        switch (playerState)
        {
            case PlayerState.Climbing:
                rb.gravityScale = 0;
                break;
            default:
                rb.gravityScale = 1;
                break;
        }

        CheckGroundedPlayerState();

        switch (playerState)
        {
            case PlayerState.Climbing:
                Climb(inputVel);
                break;
            default:
                HorizMove(inputVel);
                break;
        }

        prevState = playerState;
    }

    #region PlayerInputs

    public void OnMove(InputAction.CallbackContext context)
    {
        inputVel = context.ReadValue<Vector2>();
    }

    void CheckGroundedPlayerState()
    {
        if (!grounded || playerState == PlayerState.Climbing)
        {
            return;
        }

        if (rb.velocity.magnitude == 0)
        {
            SetPlayerState(PlayerState.Idle);
            movingTime = 0;
        }
        else if (Mathf.Abs(rb.velocity.x) > maxNormalSpeed)
        {
            SetPlayerState(PlayerState.Sprinting);
        }
        else
        {
            SetPlayerState(PlayerState.Walking);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(grounded || playerState == PlayerState.Climbing)
        {
            SetPlayerState(PlayerState.Jumping);
            Jump();
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if(context.canceled || !grounded)
        {
            maxVelocity = maxNormalSpeed;
        }
        else
        {
            maxVelocity = maxSprintSpeed;
        }
    }

    void HorizMove(Vector2 inputVel)
    {
        // This shouldn't be here. REmove to animator script
        if (Mathf.Abs(inputVel.x) > 0 && velocity.x != 0)
        {
            spriteRenderer.flipX = velocity.x < 0;
        }

        // Set Sprinting vs. Walking State and maxVelocity
        // TODO: Convert from movingTime to adding to velocity
        if (grounded)
        {
            if (Mathf.Abs(inputVel.x) > 0) // Acclerate
            {
                movingTime += Time.deltaTime * inputVel.x;
                movingTime = Mathf.Clamp(movingTime, -timeToFullAcceleration, timeToFullAcceleration);
            }
            else if (inputVel.x == 0 && movingTime > 0) // Decelerate
            {
                movingTime -= Time.deltaTime;
                movingTime = Mathf.Clamp(movingTime, 0, timeToFullAcceleration);
            }
            else if (inputVel.x == 0 && movingTime < 0) // Decelerate
            {
                movingTime += Time.deltaTime;
                movingTime = Mathf.Clamp(movingTime, -timeToFullAcceleration, 0);
            }

            normalizedTime = movingTime / timeToFullAcceleration * Mathf.PI / 2; // So that full acceleration equals Pi/2 in the Sinusoid
            velocity.x = maxVelocity * Mathf.Cos(normalizedTime - Mathf.PI / 2);
        }
        else // airborne adjustment
        {
            if (Math.Abs(rb.velocity.x) > 0) // if we haven't collided with an object in the air
            {
                movingTime += airborneAdj * Time.deltaTime * inputVel.x;
                movingTime = Mathf.Clamp(movingTime, -timeToFullAcceleration, timeToFullAcceleration);

                normalizedTime = movingTime / timeToFullAcceleration * Mathf.PI / 2; // So that full acceleration equals Pi/2 in the Sinusoid
                velocity.x = maxVelocity * Mathf.Cos(normalizedTime - Mathf.PI / 2);
            }
            else
            {
                Debug.Log("Airborne collision!");
                movingTime = 0;
            }
        }

        rb.velocity = velocity;
    }

    void Jump()
    {   
        velocity.y = jumpSpeed + Mathf.Abs(velocity.x) * horizToVertVel;
        rb.velocity = velocity;
    }

    void Climb(Vector2 inputVel)
    {
        movingTime = timeToFullAcceleration * inputVel.x;
        normalizedTime = movingTime / timeToFullAcceleration * Mathf.PI / 2; // So that full acceleration equals Pi/2 in the Sinusoid
        velocity.x = maxNormalSpeed * Mathf.Cos(normalizedTime - Mathf.PI / 2);

        velocity.y = jumpSpeed * inputVel.y;

        animator.speed = inputVel.magnitude == 0 ? 0 : 1;

        rb.velocity = velocity;
    }

    #endregion

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
        if(playerState == pState)
        {
            return;
        }

        playerState = pState;
        animator.SetTrigger(playerState.ToString());
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder") && playerState != PlayerState.Climbing)
        {
            if (inputVel.y > 0)
            {
                SetPlayerState(PlayerState.Climbing);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder") && playerState == PlayerState.Climbing)
        {
            if (grounded && inputVel.y < 0)
            {
                SetPlayerState(PlayerState.Walking);
            }
            else
            {
                SetPlayerState(PlayerState.Jumping);
            }
        }
    }
}
