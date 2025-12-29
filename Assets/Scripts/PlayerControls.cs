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

    public enum PlayerState
    {
        Idle,
        Walking,
        Sprinting,
        Jumping

    }
    private PlayerState playerState = PlayerState.Idle;
    PlayerState prevState = PlayerState.Idle;
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
        }

        prevState = playerState;
        prevXInput = inputVel.x;
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
            case PlayerState.Jumping:
                break;
        }
    }
}
