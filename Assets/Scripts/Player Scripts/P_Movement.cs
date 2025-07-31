using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;

public class P_Movement : MonoBehaviour
{

    Rigidbody2D rb;
    BoxCollider2D bounding_box;
    CapsuleCollider2D hurtbox;

    InputAction move_action;
    bool grounded = false;
    bool jumping = false;
    public float move_speed = 30f;
    public float jump_speed = 30f;
    public float jump_variability_time = 0.25f;
    bool fast_falling = false;

    public float normal_gravity = 2.5f;
    public float fastfall_gravity = 3f;
    public float fastfall_vcap = 0f;
    InputAction fastfall_action;

    bool double_jump_available = false;
    bool double_jumped = false;

    [HideInInspector]
    public float direction = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        bounding_box = gameObject.GetComponent<BoxCollider2D>();
        hurtbox = gameObject.GetComponent<CapsuleCollider2D>();

        move_action = InputSystem.actions.FindAction("Move");
        fastfall_action = InputSystem.actions.FindAction("Fastfall");
    }

    // Update is called once per frame
    void Update()
    {
        grounded = CheckGrounded();

        if (grounded)
        {
            double_jump_available = true;
        }

        if (grounded && InputSystem.actions.FindAction("Jump").WasPressedThisFrame())
        {
            jumping = true;
        }
        else if (double_jump_available && InputSystem.actions.FindAction("Jump").WasPressedThisFrame())
        {
            jumping = true;
            double_jump_available = false;
            double_jumped = true;
        }

        fast_falling = (fastfall_action.ReadValue<Vector2>().y == -1f) && !jumping;

        if (fast_falling)
        {
            if (rb.linearVelocityY > fastfall_vcap && !grounded)
            {
                rb.linearVelocityY = fastfall_vcap;
            }
            rb.gravityScale = fastfall_gravity;
        }
        else
        {
            rb.gravityScale = normal_gravity;
        }
    }

    void FixedUpdate()
    {
        float input_horizontal = move_action.ReadValue<Vector2>().x;
        float move_velocity = input_horizontal * move_speed * Time.fixedDeltaTime;
        rb.linearVelocityX = move_velocity;

        if (input_horizontal != 0f)
        {
            direction = input_horizontal;
        }

        if (jumping)
        {
            rb.linearVelocityY = jump_speed;
            if (!double_jumped)
            {
                StartCoroutine("JumpHeightVariability", jump_variability_time);
            }
            else
            {
                double_jumped = false;
                StartCoroutine("JumpHeightVariability", jump_variability_time / 2);
            }
        }
    }

    bool CheckGrounded()
    {
        RaycastHit2D boxcast_hit = Physics2D.BoxCast(
            transform.position, //origin
            new Vector2(bounding_box.size.x - 0.1f, 0.1f), //size
            0f, //angle
            Vector2.down, //cast direction
            bounding_box.size.y / 2, //cast distance
            LayerMask.GetMask("Walls") //filters for only Walls layer
        );

        if (boxcast_hit)
        {
            return true;
        }
        return false;
    }

    IEnumerator JumpHeightVariability(float variability_time)
    {
        float elapsed_time = 0f;
        while (elapsed_time < variability_time && InputSystem.actions.FindAction("Jump").IsPressed())
        {
            elapsed_time += Time.deltaTime;
            rb.linearVelocityY = jump_speed;
            yield return null;
        }
        jumping = false;
    }
}
