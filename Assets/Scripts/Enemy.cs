using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D EnemyBody;
    public float Speed = 1.0f;
    public bool MovingRight = true;
    // Start is called before the first frame update
    void Start()
    {
        EnemyBody = gameObject.GetComponent<Rigidbody2D>();
        //EnemyBody.flipX = MovingRight;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 directionTranslation = (MovingRight) ? transform.right : -transform.right;
        directionTranslation *= Time.deltaTime * Speed;
        transform.Translate(directionTranslation);
    }
}
