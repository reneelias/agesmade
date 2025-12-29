using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private Rigidbody2D rb;
    [Header("Physics")]
    [SerializeField] private float maxNormalSpeed;
    [SerializeField] private float maxSprintSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float horizToVertVel;
    [SerializeField] private float airborneAdj;

    [Header("Ground Check")]
    [SerializeField] private float raycastOffsetX;
    [SerializeField] private float raycastOffsetY;
    [SerializeField] private float raycastLength;

    float timeToFullAcceleration = 0.25f;
    float movingTime = 0;
    float prevXInput = 0;
    float normalizedTime = 0;
    float maxVelocity = 0;

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
    private Vector2 velocity;
    private bool grounded = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        switch (playerState)
        {
            // case PlayerState.Idle:
            // case PlayerState.Walking:
            // case PlayerState.Sprinting:
            //     break;
            case PlayerState.Climbing:
                Climbing();
                break;
            default:
                Movement();
                break;
        }
    }

    void Movement()
    {
        Vector2 inputVel = Vector2.zero;
        if(grounded && !GroundCheck())
        {
            animator.SetTrigger("Airborne");
            animator.speed = 1;
        }
        grounded = GroundCheck();

        if (Input.GetKey(KeyCode.RightArrow))
        {
            inputVel.x = 1;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            inputVel.x = -1;
        }
        else
        {
            inputVel.x = 0;
        }

        if(Mathf.Abs(inputVel.x) > 0 && velocity.x != 0)
        {
            spriteRenderer.flipX = velocity.x < 0;
        }

        // Set Sprinting vs. Walking State and maxVelocity
        if(grounded)
        {
            // reset movingTime if just landed or crashed into a wall
            if((prevState == PlayerState.Jumping && inputVel.x != prevXInput) || rb.velocity.x == 0)
            {
                movingTime = 0;
                velocity.x = 0;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                SetPlayerState(PlayerState.Sprinting);
            }
            else
            {
                SetPlayerState(PlayerState.Walking);
            }

            maxVelocity = playerState == PlayerState.Sprinting ? maxSprintSpeed : maxNormalSpeed;
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
            if (Mathf.Abs(inputVel.x) > 0) // Acclerate
            {
                movingTime += airborneAdj * Time.deltaTime * inputVel.x;
                normalizedTime = movingTime / timeToFullAcceleration * Mathf.PI / 2; // So that full acceleration equals Pi/2 in the Sinusoid
                velocity.x = maxVelocity * Mathf.Cos(normalizedTime - Mathf.PI / 2);
            }
        }

        Debug.Log(movingTime);
       
        velocity.y = rb.velocity.y;

        if (grounded && Input.GetKey(KeyCode.Space))
        {
            velocity.y = jumpSpeed + Mathf.Abs(velocity.x) * horizToVertVel;
            SetPlayerState(PlayerState.Jumping);
        }

        rb.velocity = velocity;

        if (velocity.magnitude == 0)
        {
            SetPlayerState(PlayerState.Idle);
            movingTime = 0;
            if (grounded)
            {
                SetPlayerState(PlayerState.Idle);
            }
        }

        prevState = playerState;
        prevXInput = inputVel.x;
    }

    void Climbing()
    {
        Vector2 inputVel = Vector2.zero;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            inputVel.x += maxNormalSpeed;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            inputVel.x -= maxNormalSpeed;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            inputVel.y += maxNormalSpeed;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            inputVel.y -= maxNormalSpeed;
        }
        animator.speed = inputVel.magnitude == 0 ? 0 : 1;

        velocity = inputVel;

        if (Input.GetKey(KeyCode.Space))
        {
            velocity.y = jumpSpeed;
            SetPlayerState(PlayerState.Walking);
        }
        rb.velocity = velocity;
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
        if(playerState == pState)
        {
            return;
        }
        playerState = pState;
        if(((int)playerState <= 2 && grounded) || (int)playerState >= 3)
        {
            animator.speed = 1;
            animator.SetTrigger(playerState.ToString());
        }
        else if((int)playerState <= 2 && !grounded)
        {
            animator.SetTrigger("Airborne");

        }

        switch (playerState)
        {
            case PlayerState.Idle:
                rb.gravityScale = 1;
                break;
            case PlayerState.Walking:
                rb.gravityScale = 1;
                break;
            case PlayerState.Sprinting:
                rb.gravityScale = 1;
                break;
            case PlayerState.Climbing:
                rb.gravityScale = 0;
                break;
            case PlayerState.Jumping:
                break;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder") && playerState != PlayerState.Climbing)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                SetPlayerState(PlayerState.Climbing);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder") && playerState == PlayerState.Climbing)
        {
            SetPlayerState(PlayerState.Walking);
        }
    }
}
