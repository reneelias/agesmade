using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private Rigidbody2D rb;
    [Header("Physics")]
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float maxMovementSpeed = 50f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float frictionRatio = .9f;
    [Header("Ground Check")]
    [SerializeField] private float raycastOffsetX = .25f;
    [SerializeField] private float raycastOffsetY = .5f;
    [SerializeField] private float raycastLength = .15f;
    private Vector2 velocity;

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
        if (Input.GetKey(KeyCode.RightArrow))
        {
            inputVel.x += movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            inputVel.x -= movementSpeed * Time.deltaTime;
        }

        velocity = rb.velocity;
        velocity += inputVel;
        velocity.x = Mathf.Clamp(velocity.x, -maxMovementSpeed, maxMovementSpeed);
        // velocity.y = rb.velocity.y;
        if(inputVel.magnitude == 0)
        {
            velocity.x *= frictionRatio;
        }

        if (GroundCheck())
        {
            if (Input.GetKey(KeyCode.Space))
            {
                velocity.y = jumpSpeed;
            }
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
}
