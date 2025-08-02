using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyLogic : MonoBehaviour
{
    public bool grounded;

    [SerializeField] private float knockbackMultiplier;
    [SerializeField] private float drag;

    private Rigidbody2D rb;
    private EnemyHealth health;
    private EnemyAnimHandler anim;

    [SerializeField] private ProjectileHandler projectile;
    [SerializeField] private float spotDistance;
    [SerializeField] private float activationDistance;
    [SerializeField] private bool manuallyActivated;
    [SerializeField] private float attackCooldown = 5f;

    private GameObject player;

    private enum State
    {
        Inactive, // not calculating logic
        Idle, // calculating logic but no player in range
        Alerted // player in range
    }

    private State awareness;
    private Vector3 distance;

    // Flying Exclusive
    private bool tracking;
    private float storedY;
    [SerializeField] private float variationY;
    private float lerpY = 0f;
    private float lerpDir = 1;
    [SerializeField] private float lerpSpeed = 0.05f;
    private float turnAngle;
    [SerializeField] private float turnSpeed;
    private int count;
    private Quaternion originalRotation;
    private Vector3 storedPos;

    // Grounded Exclusive
    [SerializeField] private float attackDistance;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float hitStun = 1;
    private float hSTimer;
    private bool attacking;
    private bool sliding;
    private float attackTimer;
    [HideInInspector] public bool isWalking;
    private CapsuleCollider2D hitbox;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        health = gameObject.GetComponent<EnemyHealth>();
        player = FindAnyObjectByType<P_Movement>().gameObject;
        storedY = gameObject.transform.position.y;
        if (grounded) hitbox = gameObject.transform.GetComponentInChildren<CapsuleCollider2D>();
        anim = gameObject.GetComponent<EnemyAnimHandler>();
        originalRotation = gameObject.transform.rotation;
    }

    private void FixedUpdate()
    {
        if (sliding) return;
        if (rb.linearVelocity.magnitude > 0)
        {
            float velMod = 1 - drag;
            if (!grounded) rb.linearVelocity *= velMod;
            else rb.linearVelocityX *= velMod;
        }
        if (attacking) return;
        attackTimer -= Time.fixedDeltaTime;
        if (grounded) Walking();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10) return;
        //Handles getting hit by the player
        P_AttackHandler playerAtk = collision.GetComponentInParent<P_AttackHandler>();
        float knockback = Vector3.Normalize(gameObject.transform.position - playerAtk.gameObject.transform.position).x * knockbackMultiplier;
        rb.linearVelocityX = knockback;

        if (!attacking) hSTimer = hitStun;
        if (grounded) isWalking = false;
        health.Damage(playerAtk.GetDamage());
    }

    private void Update()
    {
        if (tracking) gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, Quaternion.Euler(0, 0, turnAngle + 120), turnSpeed * Time.deltaTime);
        else gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, originalRotation, turnSpeed * Time.deltaTime);

        if (player != null) distance = player.transform.position - gameObject.transform.position;

        if (player == null || Vector3.Magnitude(distance) > activationDistance || (manuallyActivated && awareness == State.Inactive))
        {
            awareness = State.Inactive;
            return;
        }
        else if (Vector3.Magnitude(distance) < spotDistance) awareness = State.Alerted;
        else 
        { 
            awareness = State.Idle;
            if (grounded) isWalking = false;
        }

        if (!grounded) Floating();
    }

    private IEnumerator ShootLoop()
    {
        if (attackTimer > 0 || !(awareness == State.Alerted) || ObstructionCheck()) yield break;
        tracking = true;
        attacking = true;
        count = 0;
        storedPos = distance;
        turnAngle = Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        anim.AttackAnim();
        while (awareness == State.Alerted && count <= 15)
        {
            count++;
            if (count == 11)
            {
                ProjectileHandler proj = Instantiate(projectile, transform.position, transform.rotation);
                proj.SetDirection(Vector3.Normalize(storedPos), turnAngle);
                tracking = false;
            }
            yield return new WaitForSeconds(1/8f);
        }
        attackTimer = attackCooldown;
        attacking = false;
    }

    private bool ObstructionCheck()
    {
        RaycastHit2D castHit = Physics2D.Raycast(transform.position, Vector3.Normalize(distance), Vector3.Magnitude(distance), LayerMask.GetMask("Walls"));
        if(castHit)
        {
            return true;
        }
        return false;
    }

    private void Floating()
    {
        if (awareness == State.Alerted && !attacking) StartCoroutine(ShootLoop());

        if (lerpY == 1) lerpDir = -1f;
        else if (lerpY == 0) lerpDir = 1f;
        lerpY = Mathf.Clamp(lerpY + Time.deltaTime * lerpSpeed * lerpDir, 0, 1);
        gameObject.transform.position = new Vector2(gameObject.transform.position.x, Mathf.SmoothStep(storedY - variationY, storedY + variationY, lerpY));
    }

    private void Walking()
    {
        hSTimer -= Time.fixedDeltaTime;
        if (awareness == State.Alerted && hSTimer < 0)
        {
            isWalking = true;
            float variation = Vector3.Normalize(distance).x * moveSpeed * Time.fixedDeltaTime;
            rb.linearVelocityX = variation;

            if (Vector3.Magnitude(distance) < attackDistance) StartCoroutine(MeleeAttack());
        }
    }

    private IEnumerator MeleeAttack()
    {
        if (attackTimer > 0) yield break;
        float lungeDistance = Vector3.Normalize(distance).x;
        attacking = true;
        isWalking = false;
        hSTimer = 0; // no hitstun while attacking
        anim.AttackAnim();
        yield return new WaitForSeconds(0.5f);
        lungeDistance = Mathf.Max(Mathf.Abs(Vector3.Normalize(distance).x), Mathf.Abs(lungeDistance)) * Mathf.Sign(lungeDistance);
        StartCoroutine(Slide(400f * Time.fixedDeltaTime * 2f, 400f * Time.fixedDeltaTime * 2f, lungeDistance));
        yield return new WaitForSeconds(0.4f);
        hitbox.enabled = true;
        yield return new WaitForSeconds(0.2f);
        hitbox.enabled = false;
        yield return new WaitForSeconds(0.4f);
        attacking = false;
        attackTimer = attackCooldown;

    }

    public void ActivateEnemy()
    {
        awareness = State.Idle;
    }

    public void DeactivateEnemy()
    {
        awareness = State.Inactive;
    }

    IEnumerator Slide(float start_speed, float decceleration_per_sec, float direction)
    {
        sliding = true;
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
        sliding = false;
    }

    public float DirectionFacing()
    {
        if (distance.x == 0) return 1;
        return Mathf.Sign(distance.x);
    }

    public bool IsAttacking()
    {
        return attacking;
    }


}
