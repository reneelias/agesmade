using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerControls;

/// <summary>
/// Set animation states for player
/// </summary>
public class PlayerAnimator : MonoBehaviour
{
    private Rigidbody2D rb;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        //switch (playerState)
        //{
        //    case PlayerState.Climbing:
        //        Climb(inputVel);
        //        break;
        //    default:
        //        HorizMove(inputVel);
        //        break;
        //}
    }
}
