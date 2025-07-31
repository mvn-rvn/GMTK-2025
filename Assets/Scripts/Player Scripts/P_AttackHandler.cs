using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class P_AttackHandler : MonoBehaviour
{

    P_Movement p_movement;
    Rigidbody2D rb;
    Animator animator;

    CapsuleCollider2D g_attack_1_hitbox;
    CircleCollider2D g_attack_2_hitbox;

    Vector2 scale_original;
    Vector2 position_original;

    InputAction attack_action;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        p_movement = transform.parent.gameObject.GetComponent<P_Movement>();
        rb = transform.parent.gameObject.GetComponent<Rigidbody2D>();
        animator = GameObject.Find("Player Animations").GetComponent<Animator>();

        g_attack_1_hitbox = GameObject.Find("Grounded Attack 1").GetComponent<CapsuleCollider2D>();
        g_attack_2_hitbox = GameObject.Find("Grounded Attack 2").GetComponent<CircleCollider2D>();

        scale_original = transform.localScale;
        position_original = transform.localPosition;

        attack_action = InputSystem.actions.FindAction("Attack");
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector2(scale_original.x * p_movement.direction, scale_original.y);
        transform.localPosition = new Vector2(position_original.x * p_movement.direction, position_original.y);

        if (attack_action.WasPressedThisFrame() && p_movement.grounded)
        {
            StartCoroutine(FirstGroundedAttack());
        }
    }

    IEnumerator Slide(float start_speed, float decceleration_per_sec)
    {
        float direction = p_movement.direction;
        rb.linearVelocityX = direction * start_speed;
        float elapsed_time = 0f;

        while (elapsed_time < start_speed / decceleration_per_sec)
        {
            if (direction > 0)
            {
                rb.linearVelocityX -= decceleration_per_sec * Time.deltaTime;
                Mathf.Clamp(rb.linearVelocityX, 0f, Mathf.Infinity);
            }
            else
            {
                rb.linearVelocityX += decceleration_per_sec * Time.deltaTime;
                Mathf.Clamp(rb.linearVelocityX, -Mathf.Infinity, 0f);
            }
            elapsed_time += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator FirstGroundedAttack()
    {
        p_movement.enabled = false;
        animator.Play("AttackPrototype1");
        if (p_movement.input_horizontal != 0)
        {
            StartCoroutine(Slide(400f * Time.fixedDeltaTime * 2f, 400f * Time.fixedDeltaTime * 6f));
        }
        else
        {
            StartCoroutine(Slide(400f * Time.fixedDeltaTime * 1.5f, 400f * Time.fixedDeltaTime * 5f));
        }
        yield return new WaitForSeconds(1f / 14f);
        g_attack_1_hitbox.enabled = true;
        yield return new WaitForSeconds(2f / 14f);
        g_attack_1_hitbox.enabled = false;
        yield return new WaitForSeconds(1f / 14f);
        p_movement.enabled = true;
    }
}