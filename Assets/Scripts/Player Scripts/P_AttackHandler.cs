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
    CircleCollider2D a_attack_1_hitbox;
    CircleCollider2D a_attack_2_hitbox;
    BoxCollider2D gp_hitbox;

    Vector2 scale_original;
    Vector2 position_original;

    InputAction attack_action;

    [HideInInspector]
    public bool attacking = false;
    [HideInInspector]
    public bool combo_attack_window = false;
    bool attack_buffer_window = false;
    bool attack_input_buffer = false;

    Coroutine slide_running;

    [HideInInspector]
    public bool flipping = true;

    [SerializeField] private float baseDamage = 1f;
    [SerializeField] private float heavyDamage = 2f;
    private float damageDealt;


    InputAction fastfall_action;


    [SerializeField]
    float gp_speed = 30f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        p_movement = transform.parent.gameObject.GetComponent<P_Movement>();
        rb = transform.parent.gameObject.GetComponent<Rigidbody2D>();
        animator = GameObject.Find("Player Animations").GetComponent<Animator>();

        g_attack_1_hitbox = GameObject.Find("Grounded Attack 1").GetComponent<CapsuleCollider2D>();
        g_attack_2_hitbox = GameObject.Find("Grounded Attack 2").GetComponent<CircleCollider2D>();
        a_attack_1_hitbox = GameObject.Find("Aerial Attack 1").GetComponent<CircleCollider2D>();
        a_attack_2_hitbox = GameObject.Find("Aerial Attack 2").GetComponent<CircleCollider2D>();
        gp_hitbox = GameObject.Find("Ground Pound").GetComponent<BoxCollider2D>();

        scale_original = transform.localScale;
        position_original = transform.localPosition;

        attack_action = InputSystem.actions.FindAction("Attack");

        fastfall_action = InputSystem.actions.FindAction("Fastfall");
    }

    // Update is called once per frame
    void Update()
    {
        if (flipping)
        {
            transform.localScale = new Vector2(scale_original.x * p_movement.direction, scale_original.y);
            transform.localPosition = new Vector2(position_original.x * p_movement.direction, position_original.y);
        }

        if (attack_action.WasPressedThisFrame() && attack_buffer_window)
        {
            attack_input_buffer = true;
        }

        if (attack_action.WasPressedThisFrame() && fastfall_action.ReadValue<Vector2>().y == -1f && !attacking && !combo_attack_window && !p_movement.grounded)
        {
            StartCoroutine(GroundPound());
            return;
        }

        if (attack_action.WasPressedThisFrame() && p_movement.grounded && !attacking && !combo_attack_window)
        {
            StartCoroutine(FirstGroundedAttack());
        }
        else if (attack_action.WasPressedThisFrame() && p_movement.grounded && !attacking && combo_attack_window)
        {
            StartCoroutine(SecondGroundedAttack());
        }
        else if (attack_action.WasPressedThisFrame() && !p_movement.grounded && !attacking && !combo_attack_window)
        {
            StartCoroutine(FirstAerialAttack());
        }
        else if (attack_action.WasPressedThisFrame() && !p_movement.grounded && !attacking && combo_attack_window)
        {
            StartCoroutine(SecondAerialAttack());
        }
        else if (attack_input_buffer && !attacking && combo_attack_window)
        {
            attack_buffer_window = false;
            attack_input_buffer = false;
            StartCoroutine(BufferWait());
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
                rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, 0f, Mathf.Infinity);
                Physics.SyncTransforms();
            }
            else
            {
                rb.linearVelocityX += decceleration_per_sec * Time.deltaTime;
                rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -Mathf.Infinity, 0f);
                Physics.SyncTransforms();
            }
            elapsed_time += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator BufferWait()
    {
        while (!combo_attack_window)
        {
            yield return null;
        }

        if (p_movement.grounded)
        {
            StartCoroutine(SecondGroundedAttack());
        }
        else
        {
            StartCoroutine(SecondAerialAttack());
        }
    }

    IEnumerator FirstGroundedAttack()
    {
        damageDealt = baseDamage;
        p_movement.SetControllable(false);
        attacking = true;
        flipping = false;
        animator.Play("AttackPrototype1");
        if (p_movement.input_horizontal != 0)
        {
            slide_running = StartCoroutine(Slide(400f * Time.fixedDeltaTime * 2f, 400f * Time.fixedDeltaTime * 6f));
        }
        else
        {
            slide_running = StartCoroutine(Slide(400f * Time.fixedDeltaTime * 1.5f, 400f * Time.fixedDeltaTime * 5f));
        }
        yield return new WaitForSeconds(1f / 14f);
        g_attack_1_hitbox.enabled = true;
        yield return new WaitForSeconds(1f / 14f);
        attack_buffer_window = true;
        yield return new WaitForSeconds(1f / 14f);
        g_attack_1_hitbox.enabled = false;
        yield return new WaitForSeconds(1f / 14f);
        attacking = false;
        attack_buffer_window = false;
        combo_attack_window = true;
        yield return new WaitForSeconds(1f / 14f);
        if (!attacking)
        {
            p_movement.SetControllable(true);
            flipping = true;
        }
        yield return new WaitForSeconds(0.2f);
        combo_attack_window = false;
    }

    IEnumerator SecondGroundedAttack()
    {
        damageDealt = heavyDamage;
        p_movement.SetControllable(false);
        attacking = true;
        flipping = false;
        combo_attack_window = false;
        animator.Play("AttackPrototype2");
        if (slide_running != null)
        {
            StopCoroutine(slide_running);
        }
        StartCoroutine(Slide(400f * Time.fixedDeltaTime * 1.5f, 400f * Time.fixedDeltaTime * 5f));
        yield return new WaitForSeconds(1f / 14f);
        g_attack_2_hitbox.enabled = true;
        yield return new WaitForSeconds(1f / 14f);
        yield return new WaitForSeconds(1f / 14f);
        g_attack_2_hitbox.enabled = false;
        yield return new WaitForSeconds(1f / 14f);
        yield return new WaitForSeconds(1f / 14f);
        yield return new WaitForSeconds(1f / 14f);
        attacking = false;
        flipping = true;
        p_movement.SetControllable(true);
    }

    IEnumerator FirstAerialAttack()
    {
        p_movement.SetControllable(true);
        damageDealt = baseDamage;
        flipping = false;
        attacking = true;
        animator.Play("AttackJumpPrototype1");
        yield return new WaitForSeconds(1f / 14f);
        a_attack_1_hitbox.enabled = true;
        yield return new WaitForSeconds(1f / 14f);
        attack_buffer_window = true;
        yield return new WaitForSeconds(1f / 14f);
        a_attack_1_hitbox.enabled = false;
        yield return new WaitForSeconds(1f / 14f);
        attacking = false;
        attack_buffer_window = false;
        combo_attack_window = true;
        flipping = true;
        yield return new WaitForSeconds(1f / 14f);
        yield return new WaitForSeconds(0.2f);
        combo_attack_window = false;
        p_movement.SetControllable(true);
    }

    IEnumerator SecondAerialAttack()
    {
        p_movement.SetControllable(true);
        damageDealt = baseDamage;
        yield return null;
        flipping = false;
        attacking = true;
        combo_attack_window = false;
        animator.Play("AttackJumpPrototype2");
        yield return new WaitForSeconds(1f / 14f);
        a_attack_2_hitbox.enabled = true;
        yield return new WaitForSeconds(1f / 14f);
        yield return new WaitForSeconds(1f / 14f);
        a_attack_2_hitbox.enabled = false;
        yield return new WaitForSeconds(1f / 14f);
        yield return new WaitForSeconds(1f / 14f);
        attacking = false;
        flipping = true;
        p_movement.SetControllable(true);
    }

    IEnumerator GroundPound()
    {
        p_movement.SetControllable(false);

        damageDealt = heavyDamage;

        float original_gravity = rb.gravityScale;

        Vector2 original_velocity = rb.linearVelocity;
        rb.gravityScale = 0f;
        animator.Play("GroundPoundWindupPrototype");
        flipping = false;
        attacking = true;
        float elapsed_time = 0f;
        while (elapsed_time < 3f / 12f)
        {
            rb.linearVelocity = original_velocity / 2f;
            yield return null;
            elapsed_time += Time.deltaTime;
        }

        gp_hitbox.enabled = true;

        while (!p_movement.grounded)
        {
            rb.linearVelocity = new Vector2(0, -gp_speed);
            yield return null;
        }

        gp_hitbox.enabled = false;
        rb.gravityScale = original_gravity;

        yield return new WaitForSeconds(3f / 12f);

        attacking = false;
        flipping = true;
        p_movement.SetControllable(true);
    }

    public float GetDamage() { return damageDealt; }
}