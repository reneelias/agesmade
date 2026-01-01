using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Get Controller Input and Set Movement and Collision for Player
/// </summary>
public class PlayerControls : MonoBehaviour
{

    #region Properties
    private Rigidbody2D rb;
    [Header("Physics")]
    [SerializeField] private float maxNormalSpeed;
    [SerializeField] private float maxSprintSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float horizToVertVel;
    [SerializeField] private float airborneAdj;
    [SerializeField] private float timeToFullAccel;
    [SerializeField] private float accelerationCurvePower;

    [Header("Ground Check")]
    [SerializeField] private float raycastOffsetX;
    [SerializeField] private float raycastOffsetY;
    [SerializeField] private float raycastLength;

    private Animator animator;
    private SpriteRenderer spriteRenderer;


    public enum ControlState
    {
        Default,
        Climbing
    }
    private ControlState controlState = ControlState.Default;

    private Vector2 inputVel = Vector2.zero;

    float maxVelocity;

    private Vector2 velocity = Vector2.zero;
    public Vector2 Velocity { get => velocity; }

    private bool grounded = false;
    private bool atLadder = false;
    #endregion

    #region MonoBehaviour

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
        animator.SetBool("Grounded", grounded);

        velocity = rb.velocity;

        switch (controlState)
        {
            case ControlState.Climbing:
                Climb(inputVel);
                break;
            default:
                HorizMove(inputVel);
                // Set FlipX for playerSprite
                if (Mathf.Abs(inputVel.x) > 0 && velocity.x != 0)
                {
                    spriteRenderer.flipX = velocity.x < 0;
                }
                break;
        }

        SetRBVelocity();
    }

    #endregion

    #region OnInputActions

    public void OnMove(InputAction.CallbackContext context)
    {
        inputVel = context.ReadValue<Vector2>();
        animator.SetFloat("InputY", inputVel.y);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(grounded || (atLadder && controlState == ControlState.Climbing))
        {
            SetControlState(ControlState.Default);
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

    #endregion

    #region PlayerControls
    void HorizMove(Vector2 inputVector)
    {
        // 1 if grounded, *airborneAdj if !grounded
        float airControlFactor = grounded ? 1 : airborneAdj;

        if (inputVector.x/velocity.x > 0 || (Mathf.Abs(inputVector.x) > 0 && velocity.x == 0)) // Acclerate on Input that matches current speed vector
        {
            float inputAcceleration = Mathf.Sign(inputVector.x)  * maxVelocity * airControlFactor;
            velocity.x += GetAccelVelocity(inputAcceleration, Time.deltaTime);

            velocity.x = Mathf.Clamp(velocity.x, -maxVelocity, maxVelocity);
        }
        else if (inputVector.x / velocity.x < 0 || inputVector.x == 0) // Decelerate on input that opposes current speed vector 
        {
            float inputAcceleration = Mathf.Sign(velocity.x) * maxVelocity * airControlFactor;
            velocity.x -= GetAccelVelocity(inputAcceleration, Time.deltaTime);

            if (velocity.x > 0)
            {
                velocity.x = Mathf.Clamp(velocity.x, 0, maxVelocity);
            }
            else
            {
                velocity.x = Mathf.Clamp(velocity.x, -maxVelocity, 0);
            }

            if (Mathf.Abs(velocity.x) < maxNormalSpeed/4) // clamp at low values
            {
                velocity.x = 0;
            }
        }
    }

    /// <summary>
    /// Use Acceleration and Time to get Final Velocity.
    /// </summary>
    /// <param name="accel"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    float GetAccelVelocity(float accel, float time)
    {
        float accelRate = Mathf.Pow(timeToFullAccel, -accelerationCurvePower);

        float outputVelocity = accel * accelRate * Mathf.Pow(time, accelerationCurvePower);
        return outputVelocity;
    }

    float GetDecelVelocity(float accel, float time)
    {
        float accelRate = Mathf.Pow(timeToFullAccel, -accelerationCurvePower);
        float quickDecel = maxVelocity == maxSprintSpeed ? 12f : 8f;

        float outputVelocity = accel * quickDecel * accelRate * (Mathf.Pow(time, -accelerationCurvePower));
        return outputVelocity;
    }

    void Jump()
    {   
        velocity.y = jumpSpeed + Mathf.Abs(velocity.x) * horizToVertVel;
        SetRBVelocity();
    }

    void SetRBVelocity()
    {
        rb.velocity = velocity;
        animator.SetFloat("AbsVelocityX", Mathf.Abs(Mathf.Round(rb.velocity.x)));
    }

    void Climb(Vector2 inputVel)
    {
        velocity.x = maxNormalSpeed * inputVel.x * 50 * Time.deltaTime;
        velocity.y = jumpSpeed * inputVel.y * 50 * Time.deltaTime;

        animator.speed = inputVel.magnitude == 0 ? 0 : 1;
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

    void SetControlState(ControlState pState)
    {
        if(controlState == pState)
        {
            return;
        }

        controlState = pState;

        switch(controlState)
        {
            case ControlState.Climbing:
            {
                rb.gravityScale = 0;
                break;
            }
            default:
            {
                rb.gravityScale = 1;
                break;
            }
        }
    }

    #endregion

    #region OnTrigger

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder") && controlState != ControlState.Climbing)
        {
            atLadder = true;
            animator.SetBool("AtLadder", atLadder);

            if (inputVel.y > 0)
            {
                SetControlState(ControlState.Climbing);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            atLadder = false;
            animator.SetBool("AtLadder", atLadder);

            SetControlState(ControlState.Default);
        }
    }

    #endregion
}
